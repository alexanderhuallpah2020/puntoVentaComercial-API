using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Products;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetTopSellers
{
    internal sealed class GetTopSellersQueryHandler(IProductRepository productRepository)
        : IQueryHandler<GetTopSellersQuery, GetTopSellersResponse>
    {
        public async Task<Result<GetTopSellersResponse>> Handle(
            GetTopSellersQuery query,
            CancellationToken cancellationToken)
        {
            var items = await productRepository.GetTopSellersAsync(
                query.IdSucursal,
                query.Top,
                cancellationToken);

            var dtos = items.Select(p => new TopSellerDto(
                p.IdArticulo,
                p.Codigo,
                p.Descripcion,
                p.SiglaUnidad,
                p.PrecioVenta,
                p.StockDisponible,
                p.TipoAfectacionIgv,
                p.FlagIcbper,
                p.CantidadVendida
            )).ToList();

            return Result.Success(new GetTopSellersResponse(dtos));
        }
    }
}
