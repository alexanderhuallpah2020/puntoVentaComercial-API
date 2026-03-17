namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Commands.CreateVaultDeposit
{
    public sealed record CreateVaultDepositResponse(
        int IdDepositoBoveda,
        string NumSerie,
        string NumDocumento,
        decimal Importe);
}
