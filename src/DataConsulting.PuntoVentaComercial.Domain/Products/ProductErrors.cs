using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Products
{
    public static class ProductErrors
    {
        public static Error NotFound(int id) =>
            new("Product.NotFound", $"El artículo con ID {id} no fue encontrado.", ErrorType.NotFound);

        public static readonly Error SinResultados =
            new("Product.SinResultados", "No se encontraron artículos con los filtros indicados.", ErrorType.NotFound);
    }
}
