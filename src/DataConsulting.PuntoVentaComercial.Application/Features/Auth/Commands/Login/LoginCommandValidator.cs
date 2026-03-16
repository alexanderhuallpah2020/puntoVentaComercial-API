using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Auth.Commands.Login
{
    internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.");

            RuleFor(x => x.IdEmpresa)
                .GreaterThan(0).WithMessage("Debe indicar una empresa válida.");

            RuleFor(x => x.CodigoEstacion)
                .NotEmpty().WithMessage("El código de estación es obligatorio.");
        }
    }
}
