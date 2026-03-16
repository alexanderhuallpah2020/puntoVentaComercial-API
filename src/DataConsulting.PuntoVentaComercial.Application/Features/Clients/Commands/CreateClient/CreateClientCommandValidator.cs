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
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

            RuleFor(x => x.IdPais)
                .GreaterThan(0).WithMessage("Debe especificar el país del cliente.");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección del cliente es obligatoria.")
                .MaximumLength(200).WithMessage("La dirección no puede superar los 200 caracteres.");

            RuleFor(x => x.Telefono1)
                .MaximumLength(30).WithMessage("El teléfono no puede superar los 30 caracteres.");

            RuleFor(x => x.IdEmpresa)
                .GreaterThan(0).WithMessage("Debe indicar una empresa válida.");

            RuleFor(x => x.IdSucursal)
                .GreaterThan(0).WithMessage("Debe indicar una sucursal válida.");

            RuleFor(x => x.CreditoMaximo)
                .GreaterThanOrEqualTo(0).WithMessage("El crédito máximo no puede ser negativo.");

            // Validación específica para DNI
            When(x => x.IdDocumentoIdentidad == EDocumentoIdentidad.DNI, () =>
            {
                RuleFor(x => x.NumDocumento)
                    .NotEmpty().WithMessage("El número de DNI es obligatorio.")
                    .Length(8).WithMessage("El DNI debe tener exactamente 8 dígitos.");

                RuleFor(x => x.CodValidadorDoc)
                    .NotEmpty().WithMessage("El código verificador del DNI es obligatorio.")
                    .Length(1).WithMessage("El código verificador del DNI debe ser un solo carácter.");

                RuleFor(x => x)
                    .Must(x => !string.IsNullOrEmpty(x.NumDocumento)
                               && x.NumDocumento.Length == 8
                               && !string.IsNullOrEmpty(x.CodValidadorDoc)
                               && DniValidator.Validate(x.NumDocumento, x.CodValidadorDoc[0]))
                    .WithMessage("El DNI ingresado no es válido. Verifique el número y el código verificador.")
                    .WithName("DNI");
            });

            // Validación específica para RUC
            When(x => x.IdDocumentoIdentidad == EDocumentoIdentidad.RUC, () =>
            {
                RuleFor(x => x.NumDocumento)
                    .NotEmpty().WithMessage("El número de RUC es obligatorio.")
                    .Length(11).WithMessage("El RUC debe tener exactamente 11 dígitos.");

                RuleFor(x => x.NumDocumento)
                    .Must(ruc => RucValidator.Validate(ruc))
                    .WithMessage("El número de RUC no es válido.")
                    .When(x => !string.IsNullOrEmpty(x.NumDocumento) && x.NumDocumento.Length == 11);
            });

            // Para otros tipos de documento, solo requerir que el número no esté vacío
            When(x => x.IdDocumentoIdentidad != EDocumentoIdentidad.DNI
                      && x.IdDocumentoIdentidad != EDocumentoIdentidad.RUC
                      && x.IdDocumentoIdentidad != EDocumentoIdentidad.Todos, () =>
            {
                RuleFor(x => x.NumDocumento)
                    .NotEmpty().WithMessage("El número de documento es obligatorio.");
            });
        }
    }
}
