using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Commands.AnnulVaultDeposit
{
    public sealed record AnnulVaultDepositCommand(
        int IdDepositoBoveda,
        int IdUsuarioModificador
    ) : ICommand;
}
