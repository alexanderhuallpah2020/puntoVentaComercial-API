using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CreateSale
{
    internal sealed class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleCommandValidator()
        {
            RuleFor(x => x.NumSerie)
                .NotEmpty().WithMessage("Debe especificar la serie del documento.");

            RuleFor(x => x.IdCliente)
                .GreaterThan(0).WithMessage("Debe especificar el cliente.");

            RuleFor(x => x.NombreCliente)
                .NotEmpty().WithMessage("El nombre del cliente es obligatorio.");

            RuleFor(x => x.IdEmpresa)
                .GreaterThan(0).WithMessage("Debe especificar la empresa.");

            RuleFor(x => x.IdSucursal)
                .GreaterThan(0).WithMessage("Debe especificar la sucursal.");

            RuleFor(x => x.IdEstacion)
                .GreaterThan(0).WithMessage("Debe especificar la estación.");

            RuleFor(x => x.IdTrabajador)
                .GreaterThan(0).WithMessage("Debe especificar el vendedor.");

            RuleFor(x => x.TipoCambio)
                .GreaterThan(0).WithMessage("El tipo de cambio debe ser mayor a cero.");

            RuleFor(x => x.DescuentoGlobal)
                .GreaterThanOrEqualTo(0).WithMessage("El descuento global no puede ser negativo.");

            RuleFor(x => x.ImporteTotal)
                .GreaterThan(0).WithMessage("El importe total debe ser mayor a cero.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("La venta debe contener al menos un ítem.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.IdArticulo)
                    .GreaterThan(0).WithMessage("Cada ítem debe tener un artículo válido.");

                item.RuleFor(i => i.Cantidad)
                    .GreaterThan(0).WithMessage("La cantidad de cada ítem debe ser mayor a cero.");

                item.RuleFor(i => i.PrecioUnitario)
                    .GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo.");
            });
        }
    }
}
