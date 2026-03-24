using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Ventas;

public static class VentaErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Venta.NotFound", $"La venta con ID {id} no existe.");

    public static readonly Error SinDetalles =
        Error.Problem("Venta.SinDetalles", "Debe existir al menos un ítem con subtotal mayor a cero.");

    public static readonly Error PagoInsuficiente =
        Error.Problem("Venta.PagoInsuficiente", "El monto de pago debe ser mayor o igual al total de la venta.");

    public static readonly Error EstadoInvalidoParaAnular =
        Error.Problem("Venta.EstadoInvalidoParaAnular", "Solo se puede anular una venta en estado Emitida.");

    public static readonly Error SinTurnoActivo =
        Error.Problem("Venta.SinTurnoActivo", "Debe existir un turno de caja activo para registrar la venta.");

    public static Error ClienteNoEncontrado(int id) =>
        Error.NotFound("Venta.ClienteNoEncontrado", $"El cliente con ID {id} no existe.");

    public static readonly Error FacturaRequiereRuc =
        Error.Problem("Venta.FacturaRequiereRuc", "Para emitir una Factura, el cliente debe tener RUC válido.");

    public static Error SerieNoConfigurada(string numSerieA) =>
        Error.Problem("Venta.SerieNoConfigurada", $"La serie '{numSerieA}' no tiene correlativo configurado para esta sucursal. Configure la serie en el módulo de documentos.");
}
