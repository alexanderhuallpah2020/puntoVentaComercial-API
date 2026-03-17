namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

public interface IClienteRepository
{
    Task<Cliente?> GetByIdAsync(int idCliente, CancellationToken ct);
    Task<bool> ExistsByDocumentoAsync(int idDocIdentidad, string numDocumento, CancellationToken ct);
    Task<(IList<Cliente> Items, int Total)> SearchAsync(
        string? nombre,
        string? numDocumento,
        short? idPais,
        int? idDocIdentidad,
        int page,
        int pageSize,
        CancellationToken ct);
    Task<IList<ClienteLocal>> GetAddressesByClienteIdAsync(int idCliente, CancellationToken ct);
    Task<int> GetNextIdAsync(CancellationToken ct);
    Task<int> GetNextLocalIdAsync(CancellationToken ct);
    void Add(Cliente cliente);
}
