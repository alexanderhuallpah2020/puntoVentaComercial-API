using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Cash;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Commands.AnnulVaultDeposit
{
    internal sealed class AnnulVaultDepositCommandHandler(
        IVaultDepositRepository repository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<AnnulVaultDepositCommand>
    {
        public async Task<Result> Handle(
            AnnulVaultDepositCommand command,
            CancellationToken cancellationToken)
        {
            var deposit = await repository.GetByIdAsync(command.IdDepositoBoveda, cancellationToken);

            if (deposit is null)
                return Result.Failure(CashErrors.NotFound(command.IdDepositoBoveda));

            var result = deposit.Annul(command.IdUsuarioModificador, DateTime.Now);

            if (result.IsFailure)
                return result;

            repository.Update(deposit);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
