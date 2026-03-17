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
    public int? NumeroDocumento { get; private set; }
    public int? NroCorrelativo { get; private set; }
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
    public decimal ImporteDescuentoGlobal { get; private set; }
    public decimal PorcentajeDescuentoGlobal { get; private set; }
    public decimal ValorVenta { get; private set; }   // columna "Valorventa"
    public decimal Igv { get; private set; }           // columna "IGV"
    public decimal ValorExonerado { get; private set; }
    public decimal ImporteTotal { get; private set; }
    public decimal ImportePagado { get; private set; }
    public decimal ImporteVuelto { get; private set; }
    public decimal Redondeo { get; private set; }
    public decimal Isc { get; private set; }           // columna "ISC"
    public decimal ValorICBPER { get; private set; }
    public short IdFormaPago { get; private set; }
    public short? IdTurnoAsistencia { get; private set; }
    public short? IdSubdiario { get; private set; }
    public short IdTipoVenta { get; private set; }        // 3 = Directa (POS)
    public short FlagDescPorcentaje { get; private set; }
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

    private Venta() { }

    public static Result<Venta> Create(
        short idEmpresa,
        short idSucursal,
        short idEstacionTrabajo,
        short idSubSede,
        short idTipoDocumento,
        short? numSerie,
        int? numeroDocumento,
        int? nroCorrelativo,
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
        decimal redondeo,
        short idFormaPago,
        short flagDescPorcentaje,
        IList<VentaDetalle> detalles,
        IList<VentaPago> pagos,
        string usuarioCreador)
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
            NumeroDocumento            = numeroDocumento,
            NroCorrelativo             = nroCorrelativo,
            IdCliente                  = idCliente,
            IdTipoCliente              = idTipoCliente,
            Vendedor                   = vendedor,
            Vendedor2                  = vendedor2,
            FechaEmision               = DateTime.Now,
            FechaProceso               = DateTime.Now,
            Estado                     = "E",
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
            Redondeo                   = redondeo,
            IdFormaPago                = idFormaPago,
            IdTurnoAsistencia          = idTurnoAsistencia,
            IdTipoVenta                = 3,
            FlagDescPorcentaje         = flagDescPorcentaje,
            FlagPagoAdelantado         = 0,
            FlagDetraccion             = 0,
            UsuarioInsert              = usuarioCreador,
            FechaInsert                = DateTime.Now,
            UpdateToken                = 0
        };

        foreach (var detalle in detalles)
            venta._detalles.Add(detalle);

        foreach (var pago in pagos)
            venta._pagos.Add(pago);

        return Result.Success(venta);
    }

    public Result Anular(string usuarioModificador)
    {
        if (Estado != "E")
            return Result.Failure(VentaErrors.EstadoInvalidoParaAnular);

        Estado           = "A";
        UsuarioUpdate    = usuarioModificador;
        FechaUpdate      = DateTime.Now;
        UpdateToken++;

        return Result.Success();
    }
}
