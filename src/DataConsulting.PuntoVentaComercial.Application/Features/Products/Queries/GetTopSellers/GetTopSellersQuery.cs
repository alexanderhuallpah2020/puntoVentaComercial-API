using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetTopSellers
{
    public sealed record GetTopSellersQuery(
        int IdSucursal,
        int Top = 20
    ) : IQuery<GetTopSellersResponse>;
}
