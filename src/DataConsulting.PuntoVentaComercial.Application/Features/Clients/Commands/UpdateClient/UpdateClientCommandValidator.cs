using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.UpdateClient
{
    internal sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
    {
        public UpdateClientCommandValidator()
        {
            RuleFor(x => x.IdCliente)
                .GreaterThan(0).WithMessage("El identificador del cliente no es válido.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del cliente es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección del cliente es obligatoria.")
                .MaximumLength(200).WithMessage("La dirección no puede superar los 200 caracteres.");

            RuleFor(x => x.CreditoMaximo)
                .GreaterThanOrEqualTo(0).WithMessage("El crédito máximo no puede ser negativo.");
        }
    }
}
