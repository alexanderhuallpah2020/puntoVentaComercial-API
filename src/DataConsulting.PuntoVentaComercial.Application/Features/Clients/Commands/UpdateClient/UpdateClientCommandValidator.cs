using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.UpdateClient
{
    internal sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
    {
        public UpdateClientCommandValidator()
        {
            RuleFor(x => x.IdCliente)
                .GreaterThan(0).WithMessage("Debe especificar el cliente a actualizar.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del cliente es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede superar 150 caracteres.");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección del cliente es obligatoria.")
                .MaximumLength(300).WithMessage("La dirección no puede superar 300 caracteres.");

            RuleFor(x => x.IdSucursal)
                .GreaterThan(0).WithMessage("Debe especificar la sucursal.");

            RuleFor(x => x.IdUsuarioModificador)
                .GreaterThan(0).WithMessage("Debe especificar el usuario que realiza la modificación.");
        }
    }
}
