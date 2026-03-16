namespace DataConsulting.PuntoVentaComercial.Domain.Products
{
    public sealed record ProductTopSellerItem(
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
