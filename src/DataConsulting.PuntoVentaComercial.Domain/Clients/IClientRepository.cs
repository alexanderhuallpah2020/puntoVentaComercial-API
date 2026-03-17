namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(int idCliente, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ClientSearchResult>> SearchAsync(
            string? nombre,
            string? numDocumento,
            int? idDocumentoIdentidad,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ClientLocal>> GetLocalsAsync(
            int idCliente,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByDocumentoAsync(
            int idDocumentoIdentidad,
            string numDocumento,
            int? excludeIdCliente,
            CancellationToken cancellationToken = default);

        void Add(Client client);

        void Update(Client client);
    }

    /// <summary>
    /// Proyección liviana para resultados de búsqueda (sin cargar los Locals).
    /// </summary>
    public sealed record ClientSearchResult(
        int IdCliente,
        string Nombre,
        string NombreComercial,
        int IdDocumentoIdentidad,
        string NumDocumento,
        string EstadoCliente,
        string DireccionLocal,
        string Telefono1);
}
