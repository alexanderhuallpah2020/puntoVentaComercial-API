using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Orders;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetOrderById
{
    internal sealed class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
        : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResponse>
    {
        public async Task<Result<GetOrderByIdResponse>> Handle(
            GetOrderByIdQuery query,
            CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(query.IdPedido, cancellationToken);
            if (order is null)
                return Result.Failure<GetOrderByIdResponse>(OrderErrors.NotFound(query.IdPedido));

            var items = order.Items.Select(i => new OrderItemDto(
                i.IdDetalle,
                i.IdArticulo,
                i.Codigo,
                i.Descripcion,
                i.SiglaUnidad,
                i.Cantidad,
                i.PrecioUnitario,
                (int)i.TipoAfectacionIgv,
                i.ValorVenta,
                i.Descuento,
                i.Isc,
                i.Igv,
                i.Icbper,
                i.Subtotal)).ToList();

            return Result.Success(new GetOrderByIdResponse(
                order.IdPedido,
                order.IdEmpresa,
                order.IdSucursal,
                order.FechaEmision,
                order.TipoDocumento,
                order.NumSerie,
                order.Correlativo,
                $"{order.NumSerie}-{order.Correlativo:D8}",
                order.IdCliente,
                order.NombreCliente,
                order.IdDocumentoIdentidad,
                order.NumDocumentoCliente,
                order.DireccionCliente,
                order.FlagIGV,
                order.TipoMoneda,
                order.TipoCambio,
                order.DescuentoGlobal,
                order.ValorAfecto,
                order.ValorInafecto,
                order.ValorExonerado,
                order.TotalIsc,
                order.Igv,
                order.TotalIcbper,
                order.ImporteTotal,
                order.Estado,
                order.Observaciones,
                items));
        }
    }
}
