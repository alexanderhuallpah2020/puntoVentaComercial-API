namespace DataConsulting.PuntoVentaComercial.Domain.Ventas;

/// <summary>
/// Flags de validación cargados desde <c>dbo.Venta</c> antes de anular.
/// </summary>
public sealed record VentaAnulacionGuardsDto(
    bool TieneGuiaRemision,
    bool TieneNotaCD,
    bool TieneValorCambio,
    bool EstaContabilizada);
