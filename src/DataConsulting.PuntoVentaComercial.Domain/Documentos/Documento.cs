namespace DataConsulting.PuntoVentaComercial.Domain.Documentos;

public sealed class Documento
{
    public short IdTipoDocumento { get; private set; }
    public string? Siglas { get; private set; }
    public string? Descripcion { get; private set; }
    public string CodigoSunat { get; private set; } = default!;
    public bool FlagVenta { get; private set; }
    public bool FlagNota { get; private set; }
    public short Orden { get; private set; }
    public bool FlagCompra { get; private set; }
    public bool FlagAlmacen { get; private set; }
    public bool FlagEmision { get; private set; }
    public bool FlagMultiple { get; private set; }
    public bool FlagContable { get; private set; }
    public bool FlagNegativo { get; private set; }
    public bool FlagExterno { get; private set; }
    public bool FlagServicio { get; private set; }
    public bool FlagRUC { get; private set; }
    public byte Estado { get; private set; }
    public string FormatoNumero { get; private set; } = default!;
    public byte? TipoProceso { get; private set; }
    public short? IdUsuarioCreador { get; private set; }
    public byte? FlagSerieNumero { get; private set; }
    public bool? FlagCaja { get; private set; }
    public bool? FlagMRA { get; private set; }
    public int DocumentoFlags { get; private set; }   // columna computada
    public string SiglaSEE { get; private set; } = default!;
    public string? CodCONCAR { get; private set; }
    public byte TipoNumeracion { get; private set; }
    public byte TipoCompromisoStock { get; private set; }
    public byte TipoZofra { get; private set; }
    public int? FlagConsignacion { get; private set; }
    public int? LongSeriePle { get; private set; }
    public int? LongNumeroPle { get; private set; }
    public short? FlagLongSeriePle { get; private set; }
    public short? FlagLongNumeroPle { get; private set; }

    private Documento() { }
}
