using DataConsulting.PuntoVentaComercial.Domain.Enums;
using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CalculateSale
{
    internal sealed class CalculateSaleCommandValidator : AbstractValidator<CalculateSaleCommand>
    {
        public CalculateSaleCommandValidator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("La venta debe contener al menos un ítem.");

            RuleFor(x => x.DescuentoGlobal)
                .GreaterThanOrEqualTo(0).WithMessage("El descuento global no puede ser negativo.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.Cantidad)
                    .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");

                item.RuleFor(i => i.PrecioUnitario)
                    .GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo.");

                item.When(i => i.TipoIsc != null, () =>
                {
                    item.RuleFor(i => i)
                        .Must(i => i.TipoIsc == ETipoIsc.Especifico
                            ? i.MontoFijoIsc.HasValue && i.MontoFijoIsc > 0
                            : i.TasaIsc.HasValue && i.TasaIsc > 0)
                        .WithMessage("Los ítems con ISC deben indicar tasa (sistemas 1 y 3) o monto fijo (sistema 2).");
                });
            });
        }
    }
}
