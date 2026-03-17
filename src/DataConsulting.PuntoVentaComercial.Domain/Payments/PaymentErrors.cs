using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Payments
{
    public static class PaymentErrors
    {
        public static readonly Error SinDetalles =
            Error.Failure("Pago.SinDetalles", "El cobro debe incluir al menos una forma de pago.");

        public static readonly Error ImporteInvalido =
            Error.Failure("Pago.ImporteInvalido", "El importe pagado no puede ser cero ni negativo.");

        public static readonly Error VentaNoEncontrada =
            Error.NotFound("Pago.VentaNoEncontrada", "No se encontró la venta asociada al cobro.");

        public static Error NotFound(int idVenta) =>
            Error.NotFound("Pago.NotFound", $"No existe cobro registrado para la venta {idVenta}.");
    }
}
