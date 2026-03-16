using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Products
{
    public sealed class Product : Entity
    {
        public string Codigo { get; private set; } = string.Empty;
        public string? CodBarra { get; private set; }
        public string Descripcion { get; private set; } = string.Empty;
        public string SiglaUnidad { get; private set; } = string.Empty;
        public int IdUnidad { get; private set; }
        public decimal FactorUnd { get; private set; }
        public int FlagCompuesto { get; private set; }
        public decimal PrecioVenta { get; private set; }
        public decimal ValorVenta { get; private set; }
        public decimal StockDisponible { get; private set; }
        public int TipoAfectacionIgv { get; private set; }
        public decimal TasaIsc { get; private set; }
        public int TipoIsc { get; private set; }
        public bool FlagIcbper { get; private set; }
        public int IdClaseProducto { get; private set; }
        public byte[]? Foto { get; private set; }
        public IReadOnlyList<ProductComponent> Composicion { get; private set; } = [];

        private Product(int id) : base(id) { }
        private Product() { }

        public static Product Create(
            int id,
            string codigo,
            string? codBarra,
            string descripcion,
            string siglaUnidad,
            int idUnidad,
            decimal factorUnd,
            int flagCompuesto,
            decimal precioVenta,
            decimal valorVenta,
            decimal stockDisponible,
            int tipoAfectacionIgv,
            decimal tasaIsc,
            int tipoIsc,
            bool flagIcbper,
            int idClaseProducto,
            byte[]? foto = null)
        {
            return new Product(id)
            {
                Codigo = codigo.Trim(),
                CodBarra = codBarra?.Trim(),
                Descripcion = descripcion.Trim(),
                SiglaUnidad = siglaUnidad.Trim(),
                IdUnidad = idUnidad,
                FactorUnd = factorUnd,
                FlagCompuesto = flagCompuesto,
                PrecioVenta = precioVenta,
                ValorVenta = valorVenta,
                StockDisponible = stockDisponible,
                TipoAfectacionIgv = tipoAfectacionIgv,
                TasaIsc = tasaIsc,
                TipoIsc = tipoIsc,
                FlagIcbper = flagIcbper,
                IdClaseProducto = idClaseProducto,
                Foto = foto
            };
        }
    }
}
