namespace DataConsulting.PuntoVentaComercial.Domain.Identity
{
    public interface IWorkstationRepository
    {
        Task<Workstation?> GetByCodigoAndEmpresaAsync(
            string codigo,
            int idEmpresa,
            CancellationToken cancellationToken = default);
    }
}
