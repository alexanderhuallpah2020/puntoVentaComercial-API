using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    public interface IClientRepository
    {
        Task<Client?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<List<Client>> SearchAsync(
            string? nombre,
            string? numDocumento,
            EDocumentoIdentidad? tipoDocumento,
            int idEmpresa,
            CancellationToken ct = default);

        Task<List<ClientLocal>> GetAddressesAsync(int idCliente, CancellationToken ct = default);

        Task<bool> ExistsByDocumentoAsync(
            EDocumentoIdentidad tipo,
            string numero,
            CancellationToken ct = default);

        Task<int> InsertAsync(Client client, string telefono1, int idSucursal, CancellationToken ct = default);

        Task UpdateAsync(Client client, CancellationToken ct = default);
    }
}
