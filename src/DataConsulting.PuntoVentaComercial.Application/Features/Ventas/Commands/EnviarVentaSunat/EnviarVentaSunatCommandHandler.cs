using System.IO.Compression;
using System.Text;
using System.Xml;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Settings;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Domain.Empresas;
using DataConsulting.PuntoVentaComercial.Domain.Ventas;
using Microsoft.Extensions.Options;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.EnviarVentaSunat;

internal sealed class EnviarVentaSunatCommandHandler(
    IVentaRepository ventaRepository,
    IClienteRepository clienteRepository,
    IEmpresaFirmanteRepository empresaFirmanteRepository,
    IUblXmlGeneratorService ublService,
    IXmlSignerService signerService,
    ISunatSenderService sunatSender,
    ICurrentUserService currentUser,
    IOptions<SunatSettings> sunatOptions)
    : ICommandHandler<EnviarVentaSunatCommand, EnviarVentaSunatResponse>
{
    public async Task<Result<EnviarVentaSunatResponse>> Handle(
        EnviarVentaSunatCommand request, CancellationToken cancellationToken)
    {
        // 1. Load venta with detalles
        var venta = await ventaRepository.GetByIdAsync(request.IdVenta, cancellationToken);
        if (venta is null)
            return Result.Failure<EnviarVentaSunatResponse>(VentaErrors.NotFound(request.IdVenta));

        // 2. Load cliente
        var cliente = await clienteRepository.GetByIdAsync(venta.IdCliente, cancellationToken);
        if (cliente is null)
            return Result.Failure<EnviarVentaSunatResponse>(VentaErrors.ClienteNoEncontrado(venta.IdCliente));

        // 3. Load empresa firmante
        var firmante = await empresaFirmanteRepository.GetByIdAsync(venta.IdEmpresa, cancellationToken);
        if (firmante is null)
            return Result.Failure<EnviarVentaSunatResponse>(VentaErrors.EmpresaSinFirmante);

        // 4. Get document SUNAT code ("01"=Factura, "03"=Boleta, etc.)
        var codigoSunat = await ventaRepository.BuscarCodigoSunatAsync(request.IdVenta, cancellationToken);
        if (string.IsNullOrWhiteSpace(codigoSunat))
            return Result.Failure<EnviarVentaSunatResponse>(VentaErrors.DocumentoNoElectronico);

        // 5. Generate UBL 2.1 XML
        var xmlDoc = ublService.GenerarFactura(venta, cliente, firmante, codigoSunat);

        // 6. Sign XML with digital certificate
        var settings = sunatOptions.Value;
        if (string.IsNullOrWhiteSpace(settings.RutaCertificados))
            throw new InvalidOperationException("Sunat:RutaCertificados not configured.");
        string certPath = Path.Combine(settings.RutaCertificados, firmante.NumDocumento, firmante.NombreCertificado);
        signerService.Sign(xmlDoc, certPath, firmante.ClaveCertificado);

        // 7. Build ZIP in memory
        string xmlFileName = $"{firmante.NumDocumento}-{codigoSunat}-{venta.NumSerieA}-{venta.NumeroDocumentoA}.xml";
        string zipFileName = Path.ChangeExtension(xmlFileName, ".zip");
        byte[] zipBytes = BuildZip(xmlFileName, xmlDoc);

        // 8. Send to SUNAT
        string rucUsuario = firmante.NumDocumento + firmante.UsuarioSunat;
        bool usePruebas = settings.EsPruebas;

        var sunatResponse = await sunatSender.SendBillAsync(
            zipFileName, zipBytes, rucUsuario, firmante.ClaveSol, usePruebas, cancellationToken);

        // 9. Persist results regardless of acceptance
        byte[] xmlBytes = Encoding.UTF8.GetBytes(xmlDoc.OuterXml);
        await ventaRepository.UpdVentaArchivoXmlAsync(
            request.IdVenta,
            xmlBytes, xmlFileName,
            sunatResponse.CdrZip, sunatResponse.Accepted ? Path.ChangeExtension(zipFileName, null) + "R.zip" : null,
            currentUser.UserName, cancellationToken);

        await ventaRepository.InsVentaXmlLogAsync(
            request.IdVenta, xmlFileName, zipFileName,
            sunatResponse.Accepted ? 1 : 0,
            cancellationToken);

        // 10. Update venta status
        string estadoSunat = sunatResponse.CodigoRespuesta;
        string estadoVenta = sunatResponse.Accepted ? "A" : "A"; // venta remains active, SUNAT status tracked separately
        await ventaRepository.UpdEstadoFacturaElectronicaAsync(
            request.IdVenta, sunatResponse.CodigoRespuesta, estadoSunat, estadoVenta, cancellationToken);

        return Result.Success(new EnviarVentaSunatResponse(
            sunatResponse.CodigoRespuesta,
            sunatResponse.Descripcion));
    }

    private static byte[] BuildZip(string xmlFileName, XmlDocument xmlDoc)
    {
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entry = zip.CreateEntry(xmlFileName, CompressionLevel.Optimal);
            using var entryStream = entry.Open();
            using var writer = new XmlTextWriter(entryStream, Encoding.UTF8);
            xmlDoc.WriteTo(writer);
        }
        return ms.ToArray();
    }
}
