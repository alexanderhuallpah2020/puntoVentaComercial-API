namespace DataConsulting.PuntoVentaComercial.Domain.Empresas;

public interface IEmpresaFirmanteRepository
{
    Task<EmpresaFirmante?> GetByIdAsync(int idEmpresa, CancellationToken ct);
}
