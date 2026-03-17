using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Services.Sunat;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using DataConsulting.PuntoVentaComercial.Domain.Sales;
using DataConsulting.PuntoVentaComercial.Domain.Sunat;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Commands.SubmitSaleToSunat
{
    internal sealed class SubmitSaleToSunatCommandHandler(
        ISaleRepository saleRepository,
        ISunatSubmissionRepository submissionRepository,
        ISunatService sunatService,
        IDbConnectionFactory connectionFactory,
        IUnitOfWork unitOfWork)
        : ICommandHandler<SubmitSaleToSunatCommand, SubmitSaleToSunatResponse>
    {
        public async Task<Result<SubmitSaleToSunatResponse>> Handle(
            SubmitSaleToSunatCommand command,
            CancellationToken cancellationToken)
        {
            var sale = await saleRepository.GetByIdAsync(command.IdVenta, cancellationToken);

            if (sale is null)
                return Result.Failure<SubmitSaleToSunatResponse>(SunatErrors.VentaNoEncontrada);

            // Idempotencia: si ya fue aceptado no reenviar
            var existing = await submissionRepository.GetByIdVentaAsync(command.IdVenta, cancellationToken);
            if (existing is not null && existing.Estado == (int)ETipoEstadoSunat.Aceptado)
                return Result.Failure<SubmitSaleToSunatResponse>(SunatErrors.YaAceptada);

            // Obtener datos del emisor
            var empresa = await GetEmpresaAsync(sale.IdEmpresa);
            if (string.IsNullOrWhiteSpace(empresa.NumRUC))
                return Result.Failure<SubmitSaleToSunatResponse>(SunatErrors.EmpresaSinRuc);

            // Construir datos para el servicio SUNAT
            var items = sale.Items.Select(i => new SunatInvoiceItemData(
                Codigo: i.Codigo,
                Descripcion: i.Descripcion,
                UnidadMedidaUbl: "NIU", // TODO: mapear desde IdUnidad a código UBL
                Cantidad: i.Cantidad,
                TipoAfectacionIgv: i.TipoAfectacionIgv,
                ValorUnitario: i.PrecioUnitario,
                ValorVenta: i.ValorVenta,
                Descuento: i.Descuento,
                Isc: i.Isc,
                Igv: i.Igv,
                Icbper: i.Icbper,
                Subtotal: i.Subtotal)).ToList();

            var invoiceData = new SunatInvoiceData(
                RucEmisor: empresa.NumRUC,
                RazonSocialEmisor: empresa.RazonSocial ?? string.Empty,
                DireccionEmisor: empresa.Direccion ?? string.Empty,
                TipoDocumento: sale.TipoDocumento,
                NumSerie: sale.NumSerie,
                Correlativo: sale.Correlativo,
                FechaEmision: sale.FechaEmision,
                IdDocumentoIdentidadCliente: sale.IdDocumentoIdentidad,
                NumDocumentoCliente: sale.NumDocumentoCliente,
                NombreCliente: sale.NombreCliente,
                DireccionCliente: sale.DireccionCliente,
                ValorAfecto: sale.ValorAfecto,
                ValorInafecto: sale.ValorInafecto,
                ValorExonerado: sale.ValorExonerado,
                ValorGratuito: sale.ValorGratuito,
                TotalIsc: sale.TotalIsc,
                Igv: sale.Igv,
                TotalIcbper: sale.TotalIcbper,
                ImporteTotal: sale.ImporteTotal,
                Items: items);

            var ahora = DateTime.Now;

            // Crear o reusar el registro de envío
            SunatSubmission submission;
            if (existing is null)
            {
                submission = SunatSubmission.Create(command.IdVenta, ahora);
                submissionRepository.Add(submission);
            }
            else
            {
                submission = existing;
            }

            // Enviar a SUNAT
            SunatSubmissionResult sunatResult;
            try
            {
                sunatResult = await sunatService.SubmitInvoiceAsync(invoiceData, cancellationToken);
            }
            catch
            {
                return Result.Failure<SubmitSaleToSunatResponse>(SunatErrors.ServicioNoDisponible);
            }

            // Actualizar estado del registro
            if (sunatResult.Accepted)
                submission.MarkAsAccepted(sunatResult.CodigoRespuesta, sunatResult.MensajeRespuesta,
                    sunatResult.CdrXml, ahora);
            else
                submission.MarkAsRejected(sunatResult.CodigoRespuesta, sunatResult.MensajeRespuesta, ahora);

            submissionRepository.Update(submission);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new SubmitSaleToSunatResponse(
                command.IdVenta,
                sunatResult.Accepted,
                sunatResult.CodigoRespuesta,
                sunatResult.MensajeRespuesta,
                sunatResult.XmlHash,
                sunatResult.NumTicket));
        }

        private async Task<EmpresaRow> GetEmpresaAsync(int idEmpresa)
        {
            try
            {
                await using var connection = await connectionFactory.OpenConnectionAsync();
                return await connection.QueryFirstOrDefaultAsync<EmpresaRow>(
                    "SELECT RazonSocial, NumRUC, Direccion FROM Empresa WHERE IdEmpresa = @IdEmpresa",
                    new { IdEmpresa = idEmpresa }) ?? new EmpresaRow(null, null, null);
            }
            catch
            {
                return new EmpresaRow(null, null, null);
            }
        }

        private sealed record EmpresaRow(string? RazonSocial, string? NumRUC, string? Direccion);
    }
}
