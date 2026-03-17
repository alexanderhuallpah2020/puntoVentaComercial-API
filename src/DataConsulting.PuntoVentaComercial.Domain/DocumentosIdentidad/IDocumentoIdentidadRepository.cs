namespace DataConsulting.PuntoVentaComercial.Domain.DocumentosIdentidad;

public interface IDocumentoIdentidadRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken ct);
}
