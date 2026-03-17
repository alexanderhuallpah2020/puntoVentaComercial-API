using DataConsulting.PuntoVentaComercial.Domain.Enums;
using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Commands.CreateVaultDeposit
{
    internal sealed class CreateVaultDepositCommandValidator
        : AbstractValidator<CreateVaultDepositCommand>
    {
        public CreateVaultDepositCommandValidator()
        {
            RuleFor(x => x.IdEmpresa).GreaterThan(0);
            RuleFor(x => x.IdSucursal).GreaterThan(0);
            RuleFor(x => x.IdTrabajador).GreaterThan(0);
            RuleFor(x => x.IdIsla).GreaterThan(0);
            RuleFor(x => x.IdTurnoAsistencia).GreaterThan(0);
            RuleFor(x => x.NumSerie).NotEmpty().MaximumLength(10);
            RuleFor(x => x.NumDocumento).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Importe).GreaterThan(0);
            RuleFor(x => x.TipoCambio).GreaterThan(0);
            RuleFor(x => x.IdFormaPago).GreaterThan(0);
            RuleFor(x => x.TipoMoneda).IsInEnum();
            RuleFor(x => x.Glosa).MaximumLength(200).When(x => x.Glosa is not null);
        }
    }
}
