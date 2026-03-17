using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Commands.UpdateCliente;

public sealed class UpdateClienteCommandValidator : AbstractValidator<UpdateClienteCommand>
{
    public UpdateClienteCommandValidator()
    {
        RuleFor(x => x.IdCliente).GreaterThan(0);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IdPais).GreaterThan((short)0);
        RuleFor(x => x.DireccionLocal).NotEmpty();

        When(x => x.IdDocumentoIdentidad == (int)ETipoDocIdentidad.DNI, () =>
        {
            RuleFor(x => x.NumDocumento).NotEmpty().Length(8);
            RuleFor(x => x.CodValidadorDoc).NotEmpty();
            RuleFor(x => x)
                .Must(x => DniValidator.EsValido(x.NumDocumento!, x.CodValidadorDoc!))
                .WithMessage(x => $"El DNI {x.NumDocumento} con código validador {x.CodValidadorDoc} no es válido.");
        });

        When(x => x.IdDocumentoIdentidad == (int)ETipoDocIdentidad.RUC, () =>
        {
            RuleFor(x => x.NumDocumento)
                .NotEmpty()
                .Must(ruc => RucValidator.EsValido(ruc!))
                .WithMessage(x => $"El RUC {x.NumDocumento} no es válido.");
        });
    }
}
