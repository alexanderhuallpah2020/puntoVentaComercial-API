using DataConsulting.PuntoVentaComercial.Domain.Cash;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Queries.GetVaultDeposits
{
    public sealed record GetVaultDepositsResponse(IReadOnlyList<VaultDepositSearchResult> Items);
}
