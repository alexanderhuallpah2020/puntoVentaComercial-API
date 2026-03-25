namespace DataConsulting.PuntoVentaComercial.Domain.MovimientosAlmacen;

public sealed class MovimientoAlmacen
{
    public long IdMovimientoAlmacen { get; private set; }   // BIGINT IDENTITY
    public short IdTipoTransferencia { get; private set; }  // columna: idTipoTransferencia
    public short IdTipoDocumento { get; private set; }
    public short NumSerieT { get; private set; }
    public int NumDocumentoT { get; private set; }
    public short IdSucursal { get; private set; }
    public DateTime FechaDocumento { get; private set; }    // SMALLDATETIME — sin fracción de segundo
    public DateTime FechaMovimiento { get; private set; }
    public string EstadoTransaccion { get; private set; } = default!;
    public string? Comentario { get; private set; }
    public string UsuarioCreador { get; private set; } = default!;
    public DateTime FechaCreacion { get; private set; }
    public int? IdEntidadRef { get; private set; }
    public byte? TipoEntidad { get; private set; }
    public int IdLocacion { get; private set; }
    public int? IdInventario { get; private set; }
    public short? IdUsuarioCreador { get; private set; }

    private readonly List<DetalleMovimientoAlmacen> _detalles = [];
    public IReadOnlyCollection<DetalleMovimientoAlmacen> Detalles => _detalles.AsReadOnly();

    private MovimientoAlmacen() { }

    public static MovimientoAlmacen Create(
        short idTipoTransferencia,
        short idTipoDocumento,
        short numSerieT,
        int numDocumentoT,
        short idSucursal,
        DateTime fecha,
        string estadoTransaccion,
        string? comentario,
        int? idEntidadRef,
        byte? tipoEntidad,
        int idLocacion,
        int? idInventario,
        string usuarioCreador,
        IList<DetalleMovimientoAlmacen> detalles)
    {
        var movimiento = new MovimientoAlmacen
        {
            IdTipoTransferencia = idTipoTransferencia,
            IdTipoDocumento = idTipoDocumento,
            NumSerieT = numSerieT,
            NumDocumentoT = numDocumentoT,
            IdSucursal = idSucursal,
            FechaDocumento = fecha.Date,   // fn_supress_time_dt → solo fecha
            FechaMovimiento = fecha.Date,
            EstadoTransaccion = estadoTransaccion,
            Comentario = comentario,
            UsuarioCreador = usuarioCreador,
            FechaCreacion = DateTime.Now,
            IdEntidadRef = idEntidadRef,
            TipoEntidad = tipoEntidad,
            IdLocacion = idLocacion,
            IdInventario = idInventario,
        };

        foreach (var detalle in detalles)
            movimiento._detalles.Add(detalle);

        return movimiento;
    }
}
