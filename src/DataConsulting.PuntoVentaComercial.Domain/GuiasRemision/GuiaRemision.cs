namespace DataConsulting.PuntoVentaComercial.Domain.GuiasRemision;

public sealed class GuiaRemision
{
    public int IdGuiaRemision { get; private set; }         // INT IDENTITY
    public short IdSucursal { get; private set; }
    public short IdTipoDocumento { get; private set; }
    public short? NumSerie { get; private set; }
    public long? NumDocumento { get; private set; }
    public DateTime FechaTraslado { get; private set; }     // SMALLDATETIME
    public DateTime FechaIngreso { get; private set; }
    public byte TipoEntidad { get; private set; }
    public int IdCliente { get; private set; }
    public int? IdProveedor { get; private set; }
    public short IdMotivoGuia { get; private set; }
    public string PuntoPartida { get; private set; } = default!;
    public string PuntoLlegada { get; private set; } = default!;
    public int? IdPedido { get; private set; }
    public decimal? PesoTotal { get; private set; }
    public decimal? VolumenTotal { get; private set; }
    public long? IdMovAlmacen { get; private set; }
    public long? IdMovAlmacenTran { get; private set; }
    public string Referencia { get; private set; } = default!;
    public string Observacion { get; private set; } = default!;
    public string Nota { get; private set; } = default!;
    public string Estado { get; private set; } = default!;
    public int? IdCompra { get; private set; }
    public int? IdVentaRef { get; private set; }
    public int? IdLocalDespacho { get; private set; }
    public int? IdGuiaRemisionRef { get; private set; }
    public short IdTrabajador { get; private set; }
    public short? IdVendedor { get; private set; }
    public short IdEmpresa { get; private set; }
    public short? IdTipoMoneda { get; private set; }
    public decimal? ImporteTotal { get; private set; }
    public int? IdProceso { get; private set; }
    public byte? TipoProceso { get; private set; }
    public int? IdProceso2 { get; private set; }
    public byte? TipoProceso2 { get; private set; }
    public int? IdProceso3 { get; private set; }
    public byte? TipoProceso3 { get; private set; }
    public byte FlagDocumentoRef { get; private set; }
    public int? IdLocacion { get; private set; }
    public int? IdLocacionRef { get; private set; }
    public short? IdSucursalRef { get; private set; }
    public int? IdCentroCosto { get; private set; }
    public int? NumProyecto { get; private set; }
    public string? NombreReferencia { get; private set; }
    public byte UpdateToken { get; private set; }
    public int? IdOrdenCompra { get; private set; }
    public decimal? CantidadSalida { get; private set; }
    public decimal? CantidadMerma { get; private set; }
    public short? IdUsuarioCreador { get; private set; }
    public byte? FlagGratuito { get; private set; }
    public byte? TipoEntidadRef { get; private set; }
    public int? IdClienteRef { get; private set; }
    public int? IdProveedorRef { get; private set; }
    public int? IdTrabajadorRef { get; private set; }
    public int? IdSucursalRef2 { get; private set; }
    public int? IdAgenciaAduana { get; private set; }
    public string? NumSerieA { get; private set; }
    public string? NumDocumentoA { get; private set; }
    public string? IndicadorSunat { get; private set; }
    public string? Otros { get; private set; }
    public byte[]? CodigoQR { get; private set; }
    public int NroSecuencia { get; private set; }           // calculado en servicio: MAX(NroSecuencia)+1
    public string UsuarioIns { get; private set; } = default!;
    public DateTime FechaInsert { get; private set; }

    // Columnas calculadas por SQL Server — solo lectura, nunca se insertan:
    // TipoOperacion  AS (~[IdMotivoGuia]&(1))
    // NumSerieGen    AS (isnull(CONVERT([varchar](5),[NumSerie]),[NumSerieA]))
    // NumDocumentoGen AS (isnull(CONVERT([varchar](20),[NumDocumento]),[NumDocumentoA]))

    private readonly List<DetalleGuiaRemision> _detalles = [];
    public IReadOnlyCollection<DetalleGuiaRemision> Detalles => _detalles.AsReadOnly();

    private GuiaRemision() { }

    public static GuiaRemision Create(
        short idEmpresa,
        short idSucursal,
        short idTipoDocumento,
        short? numSerie,
        int numDocumento,
        int nroSecuencia,
        DateTime fechaTraslado,
        byte tipoEntidad,
        int idCliente,
        short idMotivoGuia,
        string puntoPartida,
        long idMovAlmacen,
        int idVentaRef,
        short idTrabajador,
        short? idVendedor,
        short? idTipoMoneda,
        decimal importeTotal,
        int? idProceso2,
        byte? tipoProceso2,
        int? idLocacion,
        string usuarioIns,
        IList<DetalleGuiaRemision> detalles)
    {
        var guia = new GuiaRemision
        {
            IdEmpresa        = idEmpresa,
            IdSucursal       = idSucursal,
            IdTipoDocumento  = idTipoDocumento,
            NumSerie         = numSerie,
            NumDocumento     = numDocumento,
            NroSecuencia     = nroSecuencia,
            FechaTraslado    = fechaTraslado.Date,
            FechaIngreso     = fechaTraslado.Date,
            TipoEntidad      = tipoEntidad,
            IdCliente        = idCliente,
            IdMotivoGuia     = idMotivoGuia,
            PuntoPartida     = puntoPartida,
            PuntoLlegada     = string.Empty,
            PesoTotal        = 0m,
            VolumenTotal     = 0m,
            IdMovAlmacen     = idMovAlmacen,
            Referencia       = string.Empty,
            Observacion      = string.Empty,
            Nota             = string.Empty,
            Estado           = "A",
            IdVentaRef       = idVentaRef,
            IdTrabajador     = idTrabajador,
            IdVendedor       = idVendedor,
            IdTipoMoneda     = idTipoMoneda,
            ImporteTotal     = importeTotal,
            IdProceso        = 0,
            TipoProceso      = 0,
            IdProceso2       = idProceso2,
            TipoProceso2     = tipoProceso2,
            IdProceso3       = 0,
            TipoProceso3     = 0,
            FlagDocumentoRef = 0,
            IdLocacion       = idLocacion,
            CantidadSalida   = 0m,
            CantidadMerma    = 0m,
            FlagGratuito     = 0,
            UpdateToken      = 0,
            IndicadorSunat   = string.Empty,
            Otros            = string.Empty,
            UsuarioIns       = usuarioIns,
            FechaInsert      = DateTime.Now,
        };

        foreach (var detalle in detalles)
            guia._detalles.Add(detalle);

        return guia;
    }
}
