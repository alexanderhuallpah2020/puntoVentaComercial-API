namespace DataConsulting.PuntoVentaComercial.Domain.GuiasRemision;

public sealed class DetalleGuiaRemision
{
    // PK compuesta: (IdGuiaRemision, CorrelativoGuia)
    public int IdGuiaRemision { get; private set; }
    public short CorrelativoGuia { get; private set; }
    public int IdArticulo { get; private set; }
    public short IdUnidad { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal CantidadRec { get; private set; }
    public decimal CostoBase { get; private set; }
    public decimal CostoAdicional { get; private set; }
    public decimal CostoUnd { get; private set; }
    public decimal Peso { get; private set; }
    public bool Igv { get; private set; }
    public short? CorrelativoPedido { get; private set; }
    public short? CorrelativoDoc { get; private set; }
    public decimal PrecioUnd { get; private set; }
    public int? IdConcepto { get; private set; }            // FK nullable a Articulo
    public decimal? PesoFinal { get; private set; }
    public int? IdSerie { get; private set; }
    public string Descripcion { get; private set; } = default!;
    public string Observacion { get; private set; } = default!;
    public short? IdMotivoTransferencia { get; private set; }
    public decimal CantidadTransferencia { get; private set; }
    public string? TextoSerie { get; private set; }
    public short? Anio { get; private set; }
    public short? IdEmpresa { get; private set; }
    public int? IdCentroCosto { get; private set; }
    public short? IdUsuarioCreador { get; private set; }

    private DetalleGuiaRemision() { }

    public static DetalleGuiaRemision Create(
        short correlativoGuia,
        short? correlativoDoc,
        int idArticulo,
        short idUnidad,
        decimal cantidad,
        decimal costoBase,
        decimal costoUnd,
        decimal precioUnd)
    {
        return new DetalleGuiaRemision
        {
            CorrelativoGuia       = correlativoGuia,
            CorrelativoDoc        = correlativoDoc,
            IdArticulo            = idArticulo,
            IdUnidad              = idUnidad,
            Cantidad              = cantidad,
            CantidadRec           = 0m,
            CostoBase             = costoBase,
            CostoAdicional        = 0m,
            CostoUnd              = costoUnd,
            Peso                  = 0m,
            Igv                   = false,
            PrecioUnd             = precioUnd,
            Descripcion           = string.Empty,
            Observacion           = string.Empty,
            CantidadTransferencia = 0m,
        };
    }
}
