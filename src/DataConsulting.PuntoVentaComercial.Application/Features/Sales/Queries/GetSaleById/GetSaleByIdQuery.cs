using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetSaleById
{
    public sealed record GetSaleByIdQuery(int IdVenta) : IQuery<GetSaleByIdResponse>;
}
