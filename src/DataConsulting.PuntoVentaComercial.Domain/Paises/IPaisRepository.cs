namespace DataConsulting.PuntoVentaComercial.Domain.Paises;

public interface IPaisRepository
{
    Task<bool> ExistsAsync(short id, CancellationToken ct);
}
