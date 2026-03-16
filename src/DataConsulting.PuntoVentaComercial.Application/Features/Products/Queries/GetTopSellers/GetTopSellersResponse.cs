namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetTopSellers
{
    public sealed record GetTopSellersResponse(IReadOnlyList<TopSellerDto> Items);

    public sealed record TopSellerDto(
        int IdArticulo,
        string Codigo,
        string Descripcion,
        string SiglaUnidad,
        decimal PrecioVenta,
        decimal StockDisponible,
        int TipoAfectacionIgv,
        bool FlagIcbper,
        int CantidadVendida
    );
}
