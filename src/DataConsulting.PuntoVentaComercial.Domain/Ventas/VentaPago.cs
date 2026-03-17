namespace DataConsulting.PuntoVentaComercial.Domain.Ventas;

public sealed class VentaPago
{
    public int IdVenta { get; private set; }
    public short IdFormaPago { get; private set; }
    public short IdTipoMoneda { get; private set; }
    public decimal Importe { get; private set; }
    public short Estado { get; private set; }
    public short UpdateToken { get; private set; }
    public short IdUsuarioCreador { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    private VentaPago() { }

    public static VentaPago Create(
        short idFormaPago,
        short idTipoMoneda,
        decimal importe,
        short idUsuarioCreador = 1)
    {
        return new VentaPago
        {
            IdFormaPago      = idFormaPago,
            IdTipoMoneda     = idTipoMoneda,
            Importe          = importe,
            Estado           = 1,
            UpdateToken      = 0,
            IdUsuarioCreador = idUsuarioCreador,
            FechaCreacion    = DateTime.Now
        };
    }
}
