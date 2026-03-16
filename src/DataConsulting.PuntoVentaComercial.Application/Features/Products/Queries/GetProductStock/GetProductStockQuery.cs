using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetProductStock
{
    public sealed record GetProductStockQuery(
        int IdArticulo,
        int IdSucursal
    ) : IQuery<GetProductStockResponse>;
}
