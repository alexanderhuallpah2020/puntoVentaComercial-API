using FluentValidation;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.CreateVenta;

public sealed class CreateVentaCommandValidator : AbstractValidator<CreateVentaCommand>
{
    public CreateVentaCommandValidator()
    {
        RuleFor(x => x.IdSucursal)
            .GreaterThan((short)0).WithMessage("Debe especificar la sucursal.");

        RuleFor(x => x.IdTipoDocumento)
            .GreaterThan((short)0).WithMessage("Debe seleccionar el tipo de documento.");

        // Serie válida: numérica (NumSerie > 0) o alfanumérica (NumSerieA no vacío)
        RuleFor(x => x)
            .Must(x => (x.NumSerie.HasValue && x.NumSerie > 0) || !string.IsNullOrWhiteSpace(x.NumSerieA))
            .WithMessage("Debe especificar la serie del documento (numérica o alfanumérica).")
            .OverridePropertyName(nameof(CreateVentaCommand.NumSerieA));

        RuleFor(x => x.IdCliente)
            .GreaterThan(0).WithMessage("Debe seleccionar un cliente.");

        RuleFor(x => x.IdVendedor)
            .GreaterThan((short)0).WithMessage("Debe especificar el vendedor.");

        RuleFor(x => x.ImporteTotal)
            .GreaterThan(0m).WithMessage("El total de la venta debe ser mayor a cero.");

        RuleFor(x => x.ImportePagado)
            .GreaterThanOrEqualTo(x => x.ImporteTotal)
            .WithMessage("El monto de pago debe ser mayor o igual al total.");

        RuleFor(x => x.Detalles)
            .NotEmpty().WithMessage("La venta debe tener al menos un ítem.")
            .Must(d => d.All(x => x.Cantidad > 0))
                .When(d => d.Detalles.Count > 0)
                .WithMessage("Todos los ítems deben tener cantidad mayor a cero.")
            .Must(d => d.All(x => x.PrecioUnitario >= 0))
                .When(d => d.Detalles.Count > 0)
                .WithMessage("El precio unitario no puede ser negativo.");

        RuleFor(x => x.Pagos)
            .NotEmpty().WithMessage("La venta debe tener al menos una forma de pago.");
    }
}
