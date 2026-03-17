using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Commands.CreateCliente;

public sealed class CreateClienteCommandValidator : AbstractValidator<CreateClienteCommand>
{
    public CreateClienteCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre del cliente es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.IdPais)
            .GreaterThan((short)0).WithMessage("Debe especificar el país.");

        RuleFor(x => x.DireccionLocal)
            .NotEmpty().WithMessage("La dirección es requerida.");

        RuleFor(x => x.IdSucursal)
            .GreaterThan((short)0).WithMessage("Debe especificar la sucursal.");

        When(x => x.IdDocumentoIdentidad == (int)ETipoDocIdentidad.DNI, () =>
        {
            RuleFor(x => x.NumDocumento)
                .NotEmpty().WithMessage("El número de DNI es requerido.")
                .Length(8).WithMessage("El DNI debe tener 8 dígitos.");

            RuleFor(x => x.CodValidadorDoc)
                .NotEmpty().WithMessage("El código validador del DNI es requerido.");

            RuleFor(x => x)
                .Must(x => DniValidator.EsValido(x.NumDocumento!, x.CodValidadorDoc!))
                .WithMessage(x => $"El DNI {x.NumDocumento} con código validador {x.CodValidadorDoc} no es válido.");
        });

        When(x => x.IdDocumentoIdentidad == (int)ETipoDocIdentidad.RUC, () =>
        {
            RuleFor(x => x.NumDocumento)
                .NotEmpty().WithMessage("El número de RUC es requerido.")
                .Must(ruc => RucValidator.EsValido(ruc!))
                .WithMessage(x => $"El RUC {x.NumDocumento} no es válido.");
        });

        When(x => x.IdDocumentoIdentidad.HasValue && x.IdDocumentoIdentidad > 0, () =>
        {
            RuleFor(x => x.NumDocumento)
                .NotEmpty().WithMessage("Debe ingresar el número de documento.");
        });
    }
}
