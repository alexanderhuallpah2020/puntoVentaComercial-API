namespace DataConsulting.PuntoVentaComercial.Domain.OperacionesPago;

public sealed class CuentaAmortizacion
{
    public short IdEmpresa { get; private set; }
    public byte TipoOperacion { get; private set; }
    public int NroOperacion { get; private set; }
    public byte TipoOperacionRef { get; private set; }  // = TipoOperacion de CuentaPendiente (2=Venta)
    public int IdOperacion { get; private set; }        // = IdVenta
    public short Secuencia { get; private set; }        // = Secuencia de CuentaPendiente
    public decimal Importe { get; private set; }
    public byte Estado { get; private set; }
    public decimal Retencion { get; private set; }
    public decimal Descuento { get; private set; }
    public decimal Detraccion { get; private set; }
    public decimal Percepcion { get; private set; }

    private CuentaAmortizacion() { }

    public static CuentaAmortizacion Create(
        short idEmpresa,
        byte tipoOperacion,
        int nroOperacion,
        byte tipoOperacionRef,
        int idOperacion,
        short secuencia,
        decimal importe)
    {
        return new CuentaAmortizacion
        {
            IdEmpresa = idEmpresa,
            TipoOperacion = tipoOperacion,
            NroOperacion = nroOperacion,
            TipoOperacionRef = tipoOperacionRef,
            IdOperacion = idOperacion,
            Secuencia = secuencia,
            Importe = importe,
            Estado = 1,
            Retencion = 0,
            Descuento = 0,
            Detraccion = 0,
            Percepcion = 0
        };
    }
}
