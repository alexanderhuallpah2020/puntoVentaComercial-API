using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Products;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetProductStock
{
    internal sealed class GetProductStockQueryHandler(IProductRepository productRepository)
        : IQueryHandler<GetProductStockQuery, GetProductStockResponse>
    {
        public async Task<Result<GetProductStockResponse>> Handle(
            GetProductStockQuery query,
            CancellationToken cancellationToken)
        {
            var stock = await productRepository.GetStockAsync(
                query.IdArticulo, query.IdSucursal, cancellationToken);

            return Result.Success(new GetProductStockResponse(
                query.IdArticulo,
                query.IdSucursal,
                stock));
        }
    }
}
