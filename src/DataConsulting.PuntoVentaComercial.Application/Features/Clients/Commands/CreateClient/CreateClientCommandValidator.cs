using DataConsulting.PuntoVentaComercial.Domain.Clients;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.CreateClient
{
    internal sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
    {
        public CreateClientCommandValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre del cliente es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede superar 150 caracteres.");

            RuleFor(x => x.IdPais)
                .GreaterThan(0).WithMessage("Debe especificar el país de origen.");

            RuleFor(x => x.IdSucursal)
                .GreaterThan(0).WithMessage("Debe especificar la sucursal.");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección del cliente es obligatoria.")
                .MaximumLength(300).WithMessage("La dirección no puede superar 300 caracteres.");

            // Cuando es DNI: 8 dígitos + CodValidadorDoc obligatorio y válido
            When(x => x.IdDocumentoIdentidad == EDocumentoIdentidad.DNI, () =>
            {
                RuleFor(x => x.NumDocumento)
                    .NotEmpty().WithMessage("Debe ingresar el número de DNI.")
                    .Length(8).WithMessage("El DNI debe tener exactamente 8 dígitos.")
                    .Matches(@"^\d{8}$").WithMessage("El DNI debe contener solo dígitos.");

                RuleFor(x => x.CodValidadorDoc)
                    .NotEmpty().WithMessage("Ingrese el código validador del DNI.")
                    .Length(1).WithMessage("El código validador debe ser 1 carácter.");

                RuleFor(x => x)
                    .Must(x => DniValidator.Validate(x.NumDocumento, x.CodValidadorDoc))
                    .WithName("DNI")
                    .WithMessage(x => $"El DNI {x.NumDocumento} con código validador '{x.CodValidadorDoc}' no es válido.");
            });

            // Cuando es RUC: 11 dígitos, válido según módulo 11
            When(x => x.IdDocumentoIdentidad == EDocumentoIdentidad.RUC, () =>
            {
                RuleFor(x => x.NumDocumento)
                    .NotEmpty().WithMessage("Debe ingresar el número de RUC.")
                    .Length(11).WithMessage("El RUC debe tener exactamente 11 dígitos.")
                    .Matches(@"^\d{11}$").WithMessage("El RUC debe contener solo dígitos.")
                    .Must(ruc => RucValidator.Validate(ruc))
                    .WithMessage(x => $"El RUC {x.NumDocumento} no es válido.");
            });

            // Otros documentos: número requerido si tipo indicado
            When(x => x.IdDocumentoIdentidad != EDocumentoIdentidad.DNI
                    && x.IdDocumentoIdentidad != EDocumentoIdentidad.RUC
                    && x.IdDocumentoIdentidad != EDocumentoIdentidad.Todos, () =>
            {
                RuleFor(x => x.NumDocumento)
                    .NotEmpty().WithMessage("Debe ingresar el número del documento.");
            });
        }
    }
}
