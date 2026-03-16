namespace DataConsulting.PuntoVentaComercial.Domain.Products
{
    public sealed record ProductPriceListItem(
        int IdArticulo,
        string Codigo,
        string? CodBarra,
        string Descripcion,
        string SiglaUnidad,
        int IdUnidad,
        decimal FactorUnd,
        int FlagCompuesto,
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
