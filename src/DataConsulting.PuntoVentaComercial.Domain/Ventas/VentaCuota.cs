namespace DataConsulting.PuntoVentaComercial.Domain.Ventas;

public sealed class VentaCuota
{
    public int IdVenta { get; private set; }
    public short Correlativo { get; private set; }
    public DateTime? FechaCuota { get; private set; }  // DATETIME NULL
    public decimal? Monto { get; private set; }         // DECIMAL(8,2) NULL
    public string? NumeroCuota { get; private set; }    // VARCHAR(8) NULL
    public decimal? Glosa { get; private set; }         // DECIMAL(8,2) NULL

    private VentaCuota() { }

    public static VentaCuota Create(
        short correlativo,
        DateTime? fechaCuota,
        decimal? monto,
        string? numeroCuota = null)
    {
        return new VentaCuota
        {
            Correlativo  = correlativo,
            FechaCuota   = fechaCuota,
            Monto        = monto,
            NumeroCuota  = numeroCuota,
            Glosa        = null
        };
    }
}
