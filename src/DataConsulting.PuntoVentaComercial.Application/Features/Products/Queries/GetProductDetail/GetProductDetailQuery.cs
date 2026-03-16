using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetProductDetail
{
    public sealed record GetProductDetailQuery(int IdArticulo) : IQuery<GetProductDetailResponse>;
}
