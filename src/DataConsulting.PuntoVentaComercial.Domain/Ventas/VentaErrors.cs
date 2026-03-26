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

    public static readonly Error YaAnulada =
        Error.Problem("Venta.YaAnulada", "El documento ya ha sido anulado.");

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

    public static readonly Error EmpresaSinFirmante =
        Error.Problem("Venta.EmpresaSinFirmante", "La empresa no tiene configurado el firmante electrónico (RUC, certificado, credenciales SUNAT).");

    public static readonly Error DocumentoNoElectronico =
        Error.Problem("Venta.DocumentoNoElectronico", "El tipo de documento no está configurado como documento electrónico en SUNAT.");

    public static readonly Error YaEnviadaASunat =
        Error.Problem("Venta.YaEnviadaASunat", "La venta ya fue enviada y aceptada por SUNAT.");

    public static Error TipoDocumentoNoSoportado(string codigoSunat) =>
        Error.Problem("Venta.TipoDocumentoNoSoportado", $"El tipo de documento con código SUNAT '{codigoSunat}' no está soportado en el envío electrónico.");

    // ── Validaciones de anulación ────────────────────────────────────────────

    public static readonly Error AceptadaEnSunat =
        Error.Problem("Venta.AceptadaEnSunat", "El documento fue enviado a SUNAT y está aceptado. No puede ser anulado.");

    public static readonly Error TieneGuiaRemision =
        Error.Problem("Venta.TieneGuiaRemision", "El documento tiene salidas de almacén asociadas.");

    public static readonly Error TieneNotaCD =
        Error.Problem("Venta.TieneNotaCD", "El documento tiene notas de crédito o débito asociadas.");

    public static readonly Error TieneValorCambio =
        Error.Problem("Venta.TieneValorCambio", "El documento está asociado a una liquidación como forma de pago.");

    public static readonly Error EstaContabilizada =
        Error.Problem("Venta.EstaContabilizada", "El documento se encuentra contabilizado. No puede ser anulado.");

    public static readonly Error TieneIngresoAlmacen =
        Error.Problem("Venta.TieneIngresoAlmacen", "El documento tiene un ingreso de almacén asociado.");

    public static readonly Error PeriodoCerrado =
        Error.Problem("Venta.PeriodoCerrado", "El período contable está cerrado para la fecha de emisión del documento.");
}
