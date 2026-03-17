using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetPrintableOrder
{
    public sealed record GetPrintableOrderQuery(int IdPedido) : IQuery<GetPrintableOrderResponse>;
}
