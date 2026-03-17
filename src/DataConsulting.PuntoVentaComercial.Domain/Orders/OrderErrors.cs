using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Orders
{
    public static class OrderErrors
    {
        public static readonly Error SinItems =
            Error.Failure("Pedido.SinItems", "El pedido debe tener al menos un ítem.");

        public static readonly Error SerieRequerida =
            Error.Failure("Pedido.SerieRequerida", "Debe especificar la serie del documento.");

        public static readonly Error CorrelativoInvalido =
            Error.Failure("Pedido.CorrelativoInvalido", "El correlativo del documento no es válido.");

        public static readonly Error SerieNoEncontrada =
            Error.NotFound("Pedido.SerieNoEncontrada", "No se encontró la serie de documento especificada.");

        public static Error NotFound(int id) =>
            Error.NotFound("Pedido.NotFound", $"El pedido {id} no existe.");

        public static Error Anulado(int id) =>
            Error.Conflict("Pedido.Anulado", $"El pedido {id} ya fue anulado.");
    }
}
