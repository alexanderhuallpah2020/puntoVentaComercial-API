namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetPriceList
{
    public sealed record GetPriceListResponse(IReadOnlyList<ProductPriceListDto> Items);

    public sealed record ProductPriceListDto(
        int IdArticulo,
        string Codigo,
        string? CodBarra,
        string Descripcion,
        string SiglaUnidad,
        int IdUnidad,
        decimal FactorUnd,
        bool EsCompuesto,
        decimal PrecioVenta,
        decimal ValorVenta,
        decimal StockDisponible,
        int TipoAfectacionIgv,
        decimal TasaIsc,
        int TipoIsc,
        bool FlagIcbper,
        int IdClaseProducto
    );
}
