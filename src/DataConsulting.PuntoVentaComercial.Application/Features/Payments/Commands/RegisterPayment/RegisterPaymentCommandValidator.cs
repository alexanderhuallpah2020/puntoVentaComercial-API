using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Payments.Commands.RegisterPayment
{
    internal sealed class RegisterPaymentCommandValidator : AbstractValidator<RegisterPaymentCommand>
    {
        public RegisterPaymentCommandValidator()
        {
            RuleFor(x => x.IdVenta)
                .GreaterThan(0).WithMessage("Debe especificar la venta a cobrar.");

            RuleFor(x => x.ImporteTotal)
                .GreaterThan(0).WithMessage("El importe total debe ser mayor a cero.");

            RuleFor(x => x.ImportePagado)
                .GreaterThan(0).WithMessage("El importe pagado debe ser mayor a cero.");

            RuleFor(x => x.Detalles)
                .NotEmpty().WithMessage("El cobro debe incluir al menos una forma de pago.");

            RuleForEach(x => x.Detalles).ChildRules(d =>
            {
                d.RuleFor(i => i.IdFormaPago)
                    .GreaterThan(0).WithMessage("Cada forma de pago debe ser válida.");

                d.RuleFor(i => i.Importe)
                    .GreaterThan(0).WithMessage("El importe de cada forma de pago debe ser mayor a cero.");

                d.RuleFor(i => i.TipoCambio)
                    .GreaterThan(0).WithMessage("El tipo de cambio debe ser mayor a cero.");
            });
        }
    }
}
