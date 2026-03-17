using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Cash
{
    public interface IVaultDepositRepository
    {
        Task<VaultDeposit?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<VaultDepositSearchResult>> SearchAsync(
            int idEmpresa,
            int idSucursal,
            int? idIsla,
            int? idTrabajador,
            EDocumento? tipoDocumento,
            string? numSerie,
            string? numDocumento,
            ETipoMoneda? tipoMoneda,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<decimal> GetAvailableCashAsync(
            int idEmpresa,
            int idSucursal,
            int? idTrabajador,
            int? idIsla,
            ETipoMoneda tipoMoneda,
            DateTime fecha,
            CancellationToken cancellationToken = default);

        void Add(VaultDeposit deposit);

        void Update(VaultDeposit deposit);
    }

    public sealed record VaultDepositSearchResult(
        int IdDepositoBoveda,
        int IdEmpresa,
        int IdSucursal,
        int IdTrabajador,
        int IdIsla,
        DateTime FechaEmision,
        int TipoDocumento,
        string NumSerie,
        string NumDocumento,
        int TipoMoneda,
        decimal Importe,
        string? Glosa,
        int Estado);
}
