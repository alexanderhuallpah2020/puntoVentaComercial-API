namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetProductStock
{
    public sealed record GetProductStockResponse(
        int IdArticulo,
        int IdSucursal,
        decimal StockDisponible
    );
}
