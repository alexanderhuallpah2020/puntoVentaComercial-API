using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Domain.Ventas;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.CreateVenta;

internal sealed class CreateVentaCommandHandler(
    IVentaRepository ventaRepository,
    IClienteRepository clienteRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateVentaCommand, int>
{
    public async Task<Result<int>> Handle(
        CreateVentaCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.GetByIdAsync(request.IdCliente, cancellationToken);
        if (cliente is null)
            return Result.Failure<int>(VentaErrors.ClienteNoEncontrado(request.IdCliente));

        int nextNumero = await ventaRepository.GetNextNumeroDocumentoAsync(
            request.IdSucursal, request.IdTipoDocumento, request.NumSerieA, cancellationToken);

        short correlativo = 1;
        var detalles = request.Detalles
            .Select(d => VentaDetalle.Create(
                correlativo++,
                d.IdArticulo,
                d.IdUnidad,
                d.DescripcionArticulo,
                d.Cantidad,
                d.PrecioUnitario,
                d.ImporteDescuento,
                d.TipoDescuento,
                d.FlagExonerado,
                d.FlagRegalo,
                d.IdTipoAfectoIGV,
                d.Isc,
                d.ValorICBPER))
            .ToList();

        var pagos = request.Pagos
            .Select(p => VentaPago.Create(p.IdFormaPago, p.IdTipoMoneda, p.Importe))
            .ToList();

        var result = Venta.Create(
            request.IdEmpresa,
            request.IdSucursal,
            request.IdEstacionTrabajo,
            request.IdSubSede,
            request.IdTipoDocumento,
            request.NumSerie,
            request.NumSerieA,
            nextNumero,
            request.IdCliente,
            request.IdTipoCliente,
            request.IdVendedor,
            request.IdVendedor2,
            request.IdTurnoAsistencia,
            request.IdTipoMoneda,
            request.TipoCambio,
            request.ValorNeto,
            request.ImporteDescuento,
            request.ImporteDescuentoGlobal,
            request.PorcentajeDescuentoGlobal,
            request.ValorVenta,
            request.Igv,
            request.ValorExonerado,
            request.Isc,
            request.ValorICBPER,
            request.ImporteTotal,
            request.ImportePagado,
            request.ImporteVuelto,
            request.RedondeoTotal,
            request.IdFormaPago,
            request.FlagDescPorcentaje,
            detalles,
            pagos,
            usuarioCreador: "SISTEMA");

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        ventaRepository.Add(result.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
