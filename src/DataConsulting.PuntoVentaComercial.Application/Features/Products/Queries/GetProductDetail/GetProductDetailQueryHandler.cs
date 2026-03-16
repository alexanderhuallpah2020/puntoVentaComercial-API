using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Products;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetProductDetail
{
    internal sealed class GetProductDetailQueryHandler(IProductRepository productRepository)
        : IQueryHandler<GetProductDetailQuery, GetProductDetailResponse>
    {
        public async Task<Result<GetProductDetailResponse>> Handle(
            GetProductDetailQuery query,
            CancellationToken cancellationToken)
        {
            var product = await productRepository.GetByIdWithComposicionAsync(
                query.IdArticulo, cancellationToken);

            if (product is null)
                return Result.Failure<GetProductDetailResponse>(ProductErrors.NotFound(query.IdArticulo));

            var composicion = product.Composicion.Select(c => new ProductComponentDto(
                c.IdArticuloComponente,
                c.Codigo,
                c.Descripcion,
                c.SiglaUnidad,
                c.Cantidad,
                c.TipoAfectacionIgv,
                c.TasaIsc,
                c.TipoIsc,
                c.FlagIcbper
            )).ToList();

            return Result.Success(new GetProductDetailResponse(
                product.Id,
                product.Codigo,
                product.CodBarra,
                product.Descripcion,
                product.SiglaUnidad,
                product.IdUnidad,
                product.FactorUnd,
                product.FlagCompuesto == 2,
                product.PrecioVenta,
                product.ValorVenta,
                product.StockDisponible,
                product.TipoAfectacionIgv,
                product.TasaIsc,
                product.TipoIsc,
                product.FlagIcbper,
                product.IdClaseProducto,
                composicion
            ));
        }
    }
}
