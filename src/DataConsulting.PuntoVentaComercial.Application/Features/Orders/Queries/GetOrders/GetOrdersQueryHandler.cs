using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Orders;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetOrders
{
    internal sealed class GetOrdersQueryHandler(IOrderRepository orderRepository)
        : IQueryHandler<GetOrdersQuery, GetOrdersResponse>
    {
        public async Task<Result<GetOrdersResponse>> Handle(
            GetOrdersQuery query,
            CancellationToken cancellationToken)
        {
            var results = await orderRepository.SearchAsync(
                query.IdEmpresa,
                query.IdSucursal,
                query.FechaDesde,
                query.FechaHasta,
                query.IdCliente,
                query.IdTrabajador,
                query.Estado,
                query.PageSize,
                cancellationToken);

            var dtos = results.Select(r => new OrderDto(
                r.IdPedido,
                r.IdEmpresa,
                r.IdSucursal,
                r.FechaEmision,
                r.TipoDocumento,
                r.NumSerie,
                r.Correlativo,
                $"{r.NumSerie}-{r.Correlativo:D8}",
                r.IdCliente,
                r.NombreCliente,
                r.NumDocumentoCliente,
                r.ImporteTotal,
                r.Estado)).ToList();

            return Result.Success(new GetOrdersResponse(dtos));
        }
    }
}
