namespace DataConsulting.PuntoVentaComercial.Domain.Ventas;

public sealed class VentaDetalle
{
    public int IdVenta { get; private set; }
    public short Correlativo { get; private set; }
    public short IdEmpresa { get; private set; }
    public int? IdArticulo { get; private set; }
    public short? IdUnidad { get; private set; }
    public string? DescripcionArticulo { get; private set; }
    public decimal? Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal? ValorUnitario { get; private set; }
    public decimal ValorVenta { get; private set; }
    public decimal ValorFacial { get; private set; }
    public decimal ImporteDescuento { get; private set; }
    public byte TipoDescuento { get; private set; }
    public bool Igv { get; private set; }
    public bool FlagExonerado { get; private set; }
    public decimal? Isc { get; private set; }
    public int? IdTipoAfectoIGV { get; private set; }
    public decimal ValorICBPER { get; private set; }
    public byte? FlagRegalo { get; private set; }
    public short? IdUsuarioCreador { get; private set; }
    public DateTime? FechaCreacion { get; private set; }

    private VentaDetalle() { }

    public static VentaDetalle Create(
        short correlativo,
        short idEmpresa,
        int idArticulo,
        short idUnidad,
        string? descripcionArticulo,
        decimal cantidad,
        decimal precioUnitario,
        decimal importeDescuento,
        byte tipoDescuento,
        bool flagExonerado,
        byte flagRegalo,
        int idTipoAfectoIGV,
        decimal isc,
        decimal valorICBPER,
        DateTime ahora,
        short idUsuarioCreador = 1)
    {
        decimal valorVenta = cantidad * precioUnitario - importeDescuento;

        return new VentaDetalle
        {
            Correlativo          = correlativo,
            IdEmpresa            = idEmpresa,
            IdArticulo           = idArticulo,
            IdUnidad             = idUnidad,
            DescripcionArticulo  = descripcionArticulo,
            Cantidad             = cantidad,
            PrecioUnitario       = precioUnitario,
            ValorUnitario        = precioUnitario,
            ValorVenta           = valorVenta,
            ValorFacial          = cantidad * precioUnitario,
            ImporteDescuento     = importeDescuento,
            TipoDescuento        = tipoDescuento,
            Igv                  = !flagExonerado,
            FlagExonerado        = flagExonerado,
            Isc                  = isc,
            IdTipoAfectoIGV      = idTipoAfectoIGV,
            ValorICBPER          = valorICBPER,
            FlagRegalo           = flagRegalo,
            IdUsuarioCreador     = idUsuarioCreador,
            FechaCreacion        = ahora
        };
    }
}
