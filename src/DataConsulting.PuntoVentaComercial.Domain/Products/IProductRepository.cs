namespace DataConsulting.PuntoVentaComercial.Domain.Products
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<ProductPriceListItem>> GetPriceListAsync(
            int idSucursal,
            int idTipoCliente,
            string? codigo,
            string? descripcion,
            bool soloConStock,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ProductTopSellerItem>> GetTopSellersAsync(
            int idSucursal,
            int top,
            CancellationToken cancellationToken = default);

        Task<Product?> GetByIdWithComposicionAsync(
            int idArticulo,
            CancellationToken cancellationToken = default);

        Task<decimal> GetStockAsync(
            int idArticulo,
            int idSucursal,
            CancellationToken cancellationToken = default);
    }
}
