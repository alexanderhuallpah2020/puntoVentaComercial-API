using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Ventas;

public sealed class Venta : Entity
{
    public short IdEmpresa { get; private set; }
    public short IdSucursal { get; private set; }
    public short IdEstacionTrabajo { get; private set; }
    public short IdSubSede { get; private set; }
    public short IdTipoDocumento { get; private set; }
    public short? NumSerie { get; private set; }
    public string? NumSerieA { get; private set; }        // VARCHAR(5)  — serie alfanumérica ej. 'F001'
    public int? NumeroDocumento { get; private set; }     // INT NULL    — para series numéricas
    public string? NumeroDocumentoA { get; private set; } // VARCHAR(20) — para series alfanuméricas ej. '000001'
    public int NroCorrelativo { get; private set; }
    public int IdCliente { get; private set; }
    public short? IdTipoCliente { get; private set; }
    public short Vendedor { get; private set; }
    public short? Vendedor2 { get; private set; }
    public DateTime FechaEmision { get; private set; }
    public DateTime FechaProceso { get; private set; }
    public string Estado { get; private set; } = default!;
    public short IdTipoMoneda { get; private set; }
    public decimal TipoCambio { get; private set; }
    public decimal ValorNeto { get; private set; }
    public decimal ImporteDescuento { get; private set; }
    public decimal? ImporteDescuentoGlobal { get; private set; }    // MONEY NULL
    public decimal? PorcentajeDescuentoGlobal { get; private set; } // MONEY NULL
    public decimal ValorVenta { get; private set; }   // columna "Valorventa"
    public decimal Igv { get; private set; }           // columna "IGV"
    public decimal ValorExonerado { get; private set; }
    public decimal ImporteTotal { get; private set; }
    public decimal? ImportePagado { get; private set; }  // MONEY NULL
    public decimal? ImporteVuelto { get; private set; }  // MONEY NULL
    public decimal? RedondeoTotal { get; private set; }  // MONEY NULL
    public decimal? Isc { get; private set; }            // ISC MONEY NULL
    public decimal ValorICBPER { get; private set; }
    public short IdFormaPago { get; private set; }
    public short? IdTurnoAsistencia { get; private set; }
    public short? IdSubdiario { get; private set; }
    public short IdTipoVenta { get; private set; }        // 3 = Directa (POS)
    public short? FlagDescPorcentaje { get; private set; }  // SMALLINT NULL
    public byte FlagPagoAdelantado { get; private set; }
    public byte FlagDetraccion { get; private set; }
    public string UsuarioInsert { get; private set; } = default!;
    public DateTime FechaInsert { get; private set; }
    public string? UsuarioUpdate { get; private set; }
    public DateTime? FechaUpdate { get; private set; }
    public byte UpdateToken { get; private set; }

    private readonly List<VentaDetalle> _detalles = [];
    public IReadOnlyCollection<VentaDetalle> Detalles => _detalles.AsReadOnly();

    private readonly List<VentaPago> _pagos = [];
    public IReadOnlyCollection<VentaPago> Pagos => _pagos.AsReadOnly();

    private readonly List<VentaCuota> _cuotas = [];
    public IReadOnlyCollection<VentaCuota> Cuotas => _cuotas.AsReadOnly();

    public VentaEmision? Emision { get; private set; }

    private Venta() { }

    public static Result<Venta> Create(
        short idEmpresa,
        short idSucursal,
        short idEstacionTrabajo,
        short idSubSede,
        short idTipoDocumento,
        short? numSerie,
        string? numSerieA,
        int? numeroDocumento,
        int nroCorrelativo,
        int idCliente,
        short? idTipoCliente,
        short vendedor,
        short? vendedor2,
        short? idTurnoAsistencia,
        short idTipoMoneda,
        decimal tipoCambio,
        decimal valorNeto,
        decimal importeDescuento,
        decimal importeDescuentoGlobal,
        decimal porcentajeDescuentoGlobal,
        decimal valorVenta,
        decimal igv,
        decimal valorExonerado,
        decimal isc,
        decimal valorICBPER,
        decimal importeTotal,
        decimal importePagado,
        decimal importeVuelto,
        decimal redondeoTotal,
        short idFormaPago,
        short flagDescPorcentaje,
        short? idSubdiario,
        IList<VentaDetalle> detalles,
        IList<VentaPago> pagos,
        IList<VentaCuota> cuotas,
        string usuarioCreador,
        DateTime ahora,
        string clienteNombre,
        string clienteDireccion,
        string? clienteDocumento,
        string observacion,
        int puntosBonus,
        string? referencias,
        string? clienteCodValidadorDoc,
        short idUsuarioCreador = 1)
    {
        if (detalles.Count == 0)
            return Result.Failure<Venta>(VentaErrors.SinDetalles);

        if (importePagado < importeTotal)
            return Result.Failure<Venta>(VentaErrors.PagoInsuficiente);

        var venta = new Venta
        {
            IdEmpresa                  = idEmpresa,
            IdSucursal                 = idSucursal,
            IdEstacionTrabajo          = idEstacionTrabajo,
            IdSubSede                  = idSubSede,
            IdTipoDocumento            = idTipoDocumento,
            NumSerie                   = numSerie,
            NumSerieA                  = numSerieA,
            // Series numéricas (NumSerie != null) → NumeroDocumento (INT)
            // Series alfanuméricas (NumSerieA)    → NumeroDocumentoA (VARCHAR, formato D6)
            NumeroDocumento            = numSerie.HasValue ? numeroDocumento : null,
            NumeroDocumentoA           = numSerie.HasValue ? null : numeroDocumento?.ToString("D6"),
            NroCorrelativo             = nroCorrelativo,
            IdCliente                  = idCliente,
            IdTipoCliente              = idTipoCliente ?? 1, // 1 = Clientes Generales (default POS)
            Vendedor                   = vendedor,
            Vendedor2                  = vendedor2,
            FechaEmision               = ahora,
            FechaProceso               = ahora,
            Estado                     = "A",
            IdTipoMoneda               = idTipoMoneda,
            TipoCambio                 = tipoCambio,
            ValorNeto                  = valorNeto,
            ImporteDescuento           = importeDescuento,
            ImporteDescuentoGlobal     = importeDescuentoGlobal,
            PorcentajeDescuentoGlobal  = porcentajeDescuentoGlobal,
            ValorVenta                 = valorVenta,
            Igv                        = igv,
            ValorExonerado             = valorExonerado,
            Isc                        = isc,
            ValorICBPER                = valorICBPER,
            ImporteTotal               = importeTotal,
            ImportePagado              = importePagado,
            ImporteVuelto              = importeVuelto,
            RedondeoTotal              = redondeoTotal,
            IdFormaPago                = idFormaPago,
            IdSubdiario                = idSubdiario,
            IdTurnoAsistencia          = idTurnoAsistencia,
            IdTipoVenta                = 3,
            FlagDescPorcentaje         = flagDescPorcentaje,
            FlagPagoAdelantado         = 0,
            FlagDetraccion             = 0,
            UsuarioInsert              = usuarioCreador,
            FechaInsert                = ahora,
            UpdateToken                = 0
        };

        foreach (var detalle in detalles)
            venta._detalles.Add(detalle);

        foreach (var pago in pagos)
            venta._pagos.Add(pago);

        foreach (var cuota in cuotas)
            venta._cuotas.Add(cuota);

        venta.Emision = VentaEmision.Create(
            clienteNombre,
            clienteDireccion,
            clienteDocumento,
            observacion,
            puntosBonus,
            referencias,
            clienteCodValidadorDoc,
            ahora,
            idUsuarioCreador);

        return Result.Success(venta);
    }

    public Result Anular(string usuarioModificador, DateTime ahora)
    {
        if (Estado != "A")
            return Result.Failure(VentaErrors.EstadoInvalidoParaAnular);

        Estado           = "E";
        UsuarioUpdate    = usuarioModificador;
        FechaUpdate      = ahora;
        UpdateToken++;

        return Result.Success();
    }
}
