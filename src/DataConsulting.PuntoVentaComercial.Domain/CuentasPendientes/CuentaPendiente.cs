namespace DataConsulting.PuntoVentaComercial.Domain.CuentasPendientes;

public sealed class CuentaPendiente
{
    public short IdEmpresa { get; private set; }
    public byte TipoOperacion { get; private set; }   // 1=Compra, 2=Venta
    public int IdOperacion { get; private set; }       // IdVenta
    public short Secuencia { get; private set; }
    public short IdTipoMoneda { get; private set; }
    public decimal Importe { get; private set; }
    public decimal Saldo { get; private set; }
    public DateTime FechaPago { get; private set; }
    public byte Estado { get; private set; }
    public int IdEntidad { get; private set; }
    public decimal Descuentos { get; private set; }
    public decimal Retenciones { get; private set; }
    public decimal SaldoRetencion { get; private set; }
    public byte UpdateToken { get; private set; }
    public decimal Detracciones { get; private set; }
    public decimal SaldoDetraccion { get; private set; }
    public byte FlagTipo { get; private set; }         // 1=Normal, 2=Dudosa
    public decimal MontoInteres { get; private set; }
    public string Glosa { get; private set; } = default!;
    public byte FlagFacturado { get; private set; }
    public short IdTipoDocumento { get; private set; }
    public decimal Percepciones { get; private set; }
    public decimal SaldoPercepcion { get; private set; }
    public int? IdOperacionRef { get; private set; }
    public byte FlagInicial { get; private set; }
    public byte FlagLiquidado { get; private set; }
    public string UsuarioCreador { get; private set; } = default!;
    public DateTime FechaCreacion { get; private set; }

    private CuentaPendiente() { }

    public static CuentaPendiente Create(
        short idEmpresa,
        byte tipoOperacion,
        int idOperacion,
        short secuencia,
        short idTipoMoneda,
        decimal importe,
        DateTime fechaPago,
        int idEntidad,
        short idTipoDocumento,
        byte flagTipo,
        string glosa,
        string usuarioCreador,
        DateTime ahora)
    {
        return new CuentaPendiente
        {
            IdEmpresa        = idEmpresa,
            TipoOperacion    = tipoOperacion,
            IdOperacion      = idOperacion,
            Secuencia        = secuencia,
            IdTipoMoneda     = idTipoMoneda,
            Importe          = importe,
            Saldo            = importe,           // saldo inicial = importe total
            FechaPago        = fechaPago,
            Estado           = 1,
            IdEntidad        = idEntidad,
            Descuentos       = 0,
            Retenciones      = 0,
            SaldoRetencion   = 0,
            UpdateToken      = 0,
            Detracciones     = 0,
            SaldoDetraccion  = 0,
            FlagTipo         = flagTipo,
            MontoInteres     = 0,
            Glosa            = glosa,
            FlagFacturado    = 1,
            IdTipoDocumento  = idTipoDocumento,
            Percepciones     = 0,
            SaldoPercepcion  = 0,
            IdOperacionRef   = null,
            FlagInicial      = 1,
            FlagLiquidado    = 0,
            UsuarioCreador   = usuarioCreador,
            FechaCreacion    = ahora
        };
    }

    public void MarcarLiquidado(string usuarioModificador)
    {
        Saldo         = 0;
        FlagLiquidado = 1;
    }
}
