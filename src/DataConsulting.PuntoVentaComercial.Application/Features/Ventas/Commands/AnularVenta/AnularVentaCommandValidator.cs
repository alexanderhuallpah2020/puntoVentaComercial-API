using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.AnularVenta;

public sealed class AnularVentaCommandValidator : AbstractValidator<AnularVentaCommand>
{
    public AnularVentaCommandValidator()
    {
        RuleFor(x => x.IdVenta)
            .GreaterThan(0).WithMessage("Debe especificar la venta a anular.");
    }
}
