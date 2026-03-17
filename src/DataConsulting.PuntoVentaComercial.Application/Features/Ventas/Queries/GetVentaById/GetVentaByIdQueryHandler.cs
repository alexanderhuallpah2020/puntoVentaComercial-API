using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Ventas;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Queries.GetVentaById;

internal sealed class GetVentaByIdQueryHandler(IVentaRepository repository)
    : IQueryHandler<GetVentaByIdQuery, GetVentaByIdResponse>
{
    public async Task<Result<GetVentaByIdResponse>> Handle(
        GetVentaByIdQuery request, CancellationToken cancellationToken)
    {
        var venta = await repository.GetByIdAsync(request.IdVenta, cancellationToken);
        if (venta is null)
            return Result.Failure<GetVentaByIdResponse>(VentaErrors.NotFound(request.IdVenta));

        return Result.Success(MapToResponse(venta));
    }

    private static GetVentaByIdResponse MapToResponse(Venta v) => new(
        v.Id,
        v.IdTipoDocumento,
        v.NumSerie,
        v.NumeroDocumento,
        v.IdCliente,
        v.Vendedor,
        v.FechaEmision,
        v.Estado,
        v.IdTipoMoneda,
        v.ValorNeto,
        v.ImporteDescuento,
        v.ValorVenta,
        v.Igv,
        v.ValorExonerado,
        v.Isc,
        v.ValorICBPER,
        v.ImporteTotal,
        v.ImportePagado,
        v.ImporteVuelto,
        v.Detalles.Select(d => new VentaDetalleResponse(
            d.Correlativo,
            d.IdArticulo,
            d.DescripcionArticulo,
            d.Cantidad,
            d.PrecioUnitario,
            d.ImporteDescuento,
            d.ValorVenta,
            d.FlagExonerado,
            d.IdTipoAfectoIGV)).ToList(),
        v.Pagos.Select(p => new VentaPagoResponse(
            p.IdFormaPago,
            p.IdTipoMoneda,
            p.Importe)).ToList());
}
