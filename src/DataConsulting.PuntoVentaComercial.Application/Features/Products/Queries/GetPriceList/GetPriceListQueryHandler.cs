using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Products;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetPriceList
{
    internal sealed class GetPriceListQueryHandler(IProductRepository productRepository)
        : IQueryHandler<GetPriceListQuery, GetPriceListResponse>
    {
        public async Task<Result<GetPriceListResponse>> Handle(
            GetPriceListQuery query,
            CancellationToken cancellationToken)
        {
            var items = await productRepository.GetPriceListAsync(
                query.IdSucursal,
                query.IdTipoCliente,
                query.Codigo,
                query.Descripcion,
                query.SoloConStock,
                cancellationToken);

            if (items.Count == 0)
                return Result.Failure<GetPriceListResponse>(ProductErrors.SinResultados);

            var dtos = items.Select(p => new ProductPriceListDto(
                p.IdArticulo,
                p.Codigo,
                p.CodBarra,
                p.Descripcion,
                p.SiglaUnidad,
                p.IdUnidad,
                p.FactorUnd,
                p.FlagCompuesto == 2,
                p.PrecioVenta,
                p.ValorVenta,
                p.StockDisponible,
                p.TipoAfectacionIgv,
                p.TasaIsc,
                p.TipoIsc,
                p.FlagIcbper,
                p.IdClaseProducto
            )).ToList();

            return Result.Success(new GetPriceListResponse(dtos));
        }
    }
}
