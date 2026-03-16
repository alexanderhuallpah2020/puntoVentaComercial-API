using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Commands.CreateSegmentoSunat
{
    internal sealed class CreateSegmentoSunatCommandValidator : AbstractValidator<CreateSegmentoSunatCommand>
    {
        public CreateSegmentoSunatCommandValidator()
        {
            RuleFor(x => x.Codigo)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .MaximumLength(10).WithMessage("El código no puede tener más de 10 caracteres.");
            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(200).WithMessage("La descripción no puede tener más de 200 caracteres.");
        }

    }
}
