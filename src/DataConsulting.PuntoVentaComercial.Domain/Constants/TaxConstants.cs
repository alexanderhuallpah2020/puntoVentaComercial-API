namespace DataConsulting.PuntoVentaComercial.Domain.Constants
{
    /// <summary>
    /// Tasas y montos fijos vigentes según normativa SUNAT (Perú).
    /// Actualizar al modificarse la legislación; no derivar desde configuración.
    /// </summary>
    public static class TaxConstants
    {
        /// <summary>Tasa IGV: 16% + 2% IPM = 18% total.</summary>
        public const decimal TasaIgv = 0.18m;

        /// <summary>Factor de conversión precio con IGV → valor de venta.</summary>
        public const decimal FactorIgv = 1.18m;

        /// <summary>Tarifa ICBPER por bolsa plástica (Ley 30884).</summary>
        public const decimal TarifaIcbper = 0.50m;

        /// <summary>Porcentaje de percepción general (SUNAT).</summary>
        public const decimal TasaPercepcion = 0.02m;

        /// <summary>Porcentaje de retención (SUNAT).</summary>
        public const decimal TasaRetencion = 0.03m;
    }
}
