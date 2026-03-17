using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Paises;

public sealed class Pais : Entity
{
    public string Nombre { get; private set; } = default!;
    public string? Estado { get; private set; }
    public string? CodSunat { get; private set; }
    public string? CodNacionalidad { get; private set; }

    private Pais() { }
}
