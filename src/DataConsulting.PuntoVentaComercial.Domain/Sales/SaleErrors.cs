using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Sales
{
    public static class SaleErrors
    {
        public static readonly Error SinItems =
            Error.Failure("Venta.SinItems", "La venta debe tener al menos un ítem.");

        public static readonly Error DescuentoExcedeTotalAfecto =
            Error.Failure("Venta.DescuentoExcedeTotalAfecto", "El descuento global no puede superar el total afecto.");

        public static readonly Error CantidadInvalida =
            Error.Failure("Venta.CantidadInvalida", "La cantidad de cada ítem debe ser mayor a cero.");

        public static readonly Error PrecioInvalido =
            Error.Failure("Venta.PrecioInvalido", "El precio unitario no puede ser negativo.");

        public static readonly Error SerieRequerida =
            Error.Failure("Venta.SerieRequerida", "Debe especificar la serie del documento.");

        public static readonly Error CorrelativoInvalido =
            Error.Failure("Venta.CorrelativoInvalido", "El correlativo del documento no es válido.");

        public static readonly Error SerieNoEncontrada =
            Error.NotFound("Venta.SerieNoEncontrada", "No se encontró la serie de documento especificada.");

        public static Error ItemIscSinTasa(int index) =>
            Error.Failure("Venta.ItemIscSinTasa", $"El ítem {index + 1} tiene ISC pero no especifica tasa ni monto fijo.");

        public static Error NotFound(int id) =>
            Error.NotFound("Venta.NotFound", $"La venta {id} no existe.");

        public static Error Anulada(int id) =>
            Error.Conflict("Venta.Anulada", $"La venta {id} ya fue anulada.");
    }
}
