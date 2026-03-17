using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetOrders
{
    public sealed record GetOrdersQuery(
        int IdEmpresa,
        int IdSucursal,
        DateTime? FechaDesde,
        DateTime? FechaHasta,
        int? IdCliente,
        int? IdTrabajador,
        int? Estado,
        int PageSize = 100
    ) : IQuery<GetOrdersResponse>;
}
