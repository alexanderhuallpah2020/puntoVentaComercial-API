using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetOrderById
{
    public sealed record GetOrderByIdQuery(int IdPedido) : IQuery<GetOrderByIdResponse>;
}
