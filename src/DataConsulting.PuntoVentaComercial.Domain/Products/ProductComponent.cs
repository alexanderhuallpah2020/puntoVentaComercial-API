namespace DataConsulting.PuntoVentaComercial.Domain.Products
{
    public sealed class ProductComponent
    {
        public int IdArticuloComponente { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public string Descripcion { get; init; } = string.Empty;
        public string SiglaUnidad { get; init; } = string.Empty;
        public decimal Cantidad { get; init; }
        public int TipoAfectacionIgv { get; init; }
        public decimal TasaIsc { get; init; }
        public int TipoIsc { get; init; }
        public bool FlagIcbper { get; init; }
    }
}
