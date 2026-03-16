namespace DataConsulting.PuntoVentaComercial.Domain.Identity
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAndEmpresaAsync(
            string username,
            int idEmpresa,
            CancellationToken cancellationToken = default);

        Task<List<string>> GetPoliciesAsync(
            int idUsuario,
            CancellationToken cancellationToken = default);
    }
}
