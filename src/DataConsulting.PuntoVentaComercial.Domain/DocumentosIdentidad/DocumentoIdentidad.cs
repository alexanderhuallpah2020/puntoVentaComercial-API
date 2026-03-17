using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.DocumentosIdentidad;

public sealed class DocumentoIdentidad : Entity
{
    public string Nombre { get; private set; } = default!;
    public int Longitud { get; private set; }
    public short OrdenEfecto { get; private set; }
    public byte? ValorEntero { get; private set; }
    public short? IdPais { get; private set; }
    public string? MaskDocumento { get; private set; }
    public byte? Estado { get; private set; }
    public string? CodConcar { get; private set; }
    public string? CodSunat { get; private set; }

    private DocumentoIdentidad() { }
}
