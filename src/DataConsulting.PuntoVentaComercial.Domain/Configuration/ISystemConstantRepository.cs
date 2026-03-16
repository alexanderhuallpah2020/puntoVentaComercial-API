namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    public interface ISystemConstantRepository
    {
        Task<IReadOnlyList<SystemConstant>> GetConstantsAsync(
            int idEmpresa,
            int idSucursal,
            CancellationToken cancellationToken = default);
    }
}
