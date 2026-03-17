using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Sales
{
    public sealed class SaleItem : Entity
    {
        // ORM constructor
        private SaleItem() { }

        public int IdDetalle => Id;
        public int IdVenta { get; private set; }
        public int IdArticulo { get; private set; }
        public string Codigo { get; private set; } = string.Empty;
        public string Descripcion { get; private set; } = string.Empty;
        public string SiglaUnidad { get; private set; } = string.Empty;
        public int IdUnidad { get; private set; }
        public decimal Cantidad { get; private set; }
        public decimal PrecioUnitario { get; private set; }
        public ETipoAfectacionIgv TipoAfectacionIgv { get; private set; }
        public decimal ValorVenta { get; private set; }
        public decimal Descuento { get; private set; }
        public decimal ValorVentaNeto { get; private set; }
        public decimal Isc { get; private set; }
        public decimal Igv { get; private set; }
        public decimal Icbper { get; private set; }
        public decimal Subtotal { get; private set; }
        public int IdClaseProducto { get; private set; }
        public int IdTipoCliente { get; private set; }
        public int Estado { get; private set; } = 1;

        public static SaleItem Create(
            int idVenta,
            int idArticulo,
            string codigo,
            string descripcion,
            string siglaUnidad,
            int idUnidad,
            decimal cantidad,
            decimal precioUnitario,
            ETipoAfectacionIgv tipoAfectacionIgv,
            decimal valorVenta,
            decimal descuento,
            decimal isc,
            decimal igv,
            decimal icbper,
            decimal subtotal,
            int idClaseProducto,
            int idTipoCliente)
        {
            return new SaleItem
            {
                IdVenta = idVenta,
                IdArticulo = idArticulo,
                Codigo = codigo?.Trim() ?? string.Empty,
                Descripcion = descripcion?.Trim() ?? string.Empty,
                SiglaUnidad = siglaUnidad?.Trim() ?? string.Empty,
                IdUnidad = idUnidad,
                Cantidad = cantidad,
                PrecioUnitario = precioUnitario,
                TipoAfectacionIgv = tipoAfectacionIgv,
                ValorVenta = valorVenta,
                Descuento = descuento,
                ValorVentaNeto = valorVenta - descuento,
                Isc = isc,
                Igv = igv,
                Icbper = icbper,
                Subtotal = subtotal,
                IdClaseProducto = idClaseProducto,
                IdTipoCliente = idTipoCliente,
                Estado = 1
            };
        }
    }
}
