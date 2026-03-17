using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Cash;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Commands.CreateVaultDeposit
{
    internal sealed class CreateVaultDepositCommandHandler(
        IVaultDepositRepository repository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<CreateVaultDepositCommand, CreateVaultDepositResponse>
    {
        public async Task<Result<CreateVaultDepositResponse>> Handle(
            CreateVaultDepositCommand command,
            CancellationToken cancellationToken)
        {
            var result = VaultDeposit.Create(
                command.IdEmpresa,
                command.IdSucursal,
                command.IdTrabajador,
                command.IdIsla,
                command.IdTurnoAsistencia,
                command.TipoDocumento,
                command.NumSerie,
                command.NumDocumento,
                command.TipoMoneda,
                command.TipoCambio,
                command.Importe,
                command.IdFormaPago,
                command.Glosa,
                command.IdUsuarioCreador,
                DateTime.Now);

            if (result.IsFailure)
                return Result.Failure<CreateVaultDepositResponse>(result.Error);

            repository.Add(result.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new CreateVaultDepositResponse(
                result.Value.IdDepositoBoveda,
                result.Value.NumSerie,
                result.Value.NumDocumento,
                result.Value.Importe));
        }
    }
}
