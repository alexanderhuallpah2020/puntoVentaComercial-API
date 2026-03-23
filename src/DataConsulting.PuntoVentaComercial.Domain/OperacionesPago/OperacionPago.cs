namespace DataConsulting.PuntoVentaComercial.Domain.OperacionesPago;

public sealed class OperacionPago
{
    public short IdEmpresa { get; private set; }
    public byte TipoOperacion { get; private set; }    // 1=Compra, 2=Venta
    public int NroOperacion { get; private set; }      // generado con MAX+1 antes de insertar
    public DateTime FechaEmision { get; private set; }
    public short IdTipoMoneda { get; private set; }
    public decimal ImporteTotal { get; private set; }
    public decimal ImportePago { get; private set; }
    public short IdSucursal { get; private set; }
    public short IdTrabajador { get; private set; }
    public short IdEstacion { get; private set; }
    public byte Estado { get; private set; }
    public decimal TipoCambio { get; private set; }
    public decimal Retenciones { get; private set; }
    public decimal Descuentos { get; private set; }
    public int IdEntidad { get; private set; }
    public string Observaciones { get; private set; } = default!;
    public byte UpdateToken { get; private set; }
    public short? IdTurnoAsistencia { get; private set; }
    public int IdDocumentoRef { get; private set; }
    public decimal Detracciones { get; private set; }
    public decimal Percepciones { get; private set; }
    public short? IdTipoDocumentoRef { get; private set; }
    public int? IdConceptoPago { get; private set; }
    public string NumSerieDocumentoRef { get; private set; } = default!;
    public int? IdLiquidacion { get; private set; }
    public int? IdCajaChica { get; private set; }
    public byte EstadoContable { get; private set; }
    public byte? FlagRendido { get; private set; }
    public int? IdRendicionCajaChica { get; private set; }
    public short? IdConceptoCtaCte { get; private set; }
    public string UsuarioInsert { get; private set; } = default!;
    public DateTime FechaInsert { get; private set; }

    private readonly List<OperacionPagoDetalle> _detalles = [];
    public IReadOnlyCollection<OperacionPagoDetalle> Detalles => _detalles.AsReadOnly();

    private readonly List<CuentaAmortizacion> _amortizaciones = [];
    public IReadOnlyCollection<CuentaAmortizacion> Amortizaciones => _amortizaciones.AsReadOnly();

    private OperacionPago() { }

    public static OperacionPago Create(
        short idEmpresa,
        byte tipoOperacion,
        int nroOperacion,
        DateTime fechaEmision,
        short idTipoMoneda,
        decimal importeTotal,
        short idSucursal,
        short idTrabajador,
        short idEstacion,
        decimal tipoCambio,
        int idEntidad,
        string observaciones,
        short? idTurnoAsistencia,
        byte estadoContable,
        string usuarioCreador,
        IList<OperacionPagoDetalle> detalles,
        IList<CuentaAmortizacion> amortizaciones)
    {
        var operacion = new OperacionPago
        {
            IdEmpresa            = idEmpresa,
            TipoOperacion        = tipoOperacion,
            NroOperacion         = nroOperacion,
            FechaEmision         = fechaEmision,
            IdTipoMoneda         = idTipoMoneda,
            ImporteTotal         = importeTotal,
            ImportePago          = importeTotal,
            IdSucursal           = idSucursal,
            IdTrabajador         = idTrabajador,
            IdEstacion           = idEstacion,
            Estado               = 1,
            TipoCambio           = tipoCambio,
            Retenciones          = 0,
            Descuentos           = 0,
            IdEntidad            = idEntidad,
            Observaciones        = observaciones,
            UpdateToken          = 0,
            IdTurnoAsistencia    = idTurnoAsistencia,
            IdDocumentoRef       = 0,
            Detracciones         = 0,
            Percepciones         = 0,
            IdTipoDocumentoRef   = null,
            IdConceptoPago       = null,
            NumSerieDocumentoRef = "",
            IdLiquidacion        = null,
            IdCajaChica          = null,
            EstadoContable       = estadoContable,
            FlagRendido          = null,
            IdRendicionCajaChica = null,
            IdConceptoCtaCte     = null,
            UsuarioInsert        = usuarioCreador,
            FechaInsert          = DateTime.Now
        };

        foreach (var detalle in detalles)
            operacion._detalles.Add(detalle);

        foreach (var amortizacion in amortizaciones)
            operacion._amortizaciones.Add(amortizacion);

        return operacion;
    }
}
