namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetProductDetail
{
    public sealed record GetProductDetailResponse(
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
        int IdClaseProducto,
        IReadOnlyList<ProductComponentDto> Composicion
    );

    public sealed record ProductComponentDto(
        int IdArticuloComponente,
        string Codigo,
        string Descripcion,
        string SiglaUnidad,
        decimal Cantidad,
        int TipoAfectacionIgv,
        decimal TasaIsc,
        int TipoIsc,
        bool FlagIcbper
    );
}
