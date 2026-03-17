using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Services.Sales;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Sales;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CalculateSale
{
    internal sealed class CalculateSaleCommandHandler
        : ICommandHandler<CalculateSaleCommand, CalculateSaleResponse>
    {
        private readonly ISaleCalculationService _calculationService;

        public CalculateSaleCommandHandler(ISaleCalculationService calculationService)
        {
            _calculationService = calculationService;
        }

        public Task<Result<CalculateSaleResponse>> Handle(
            CalculateSaleCommand command,
            CancellationToken cancellationToken)
        {
            if (command.Items is null || command.Items.Count == 0)
                return Task.FromResult(Result.Failure<CalculateSaleResponse>(SaleErrors.SinItems));

            if (command.Items.Any(i => i.Cantidad <= 0))
                return Task.FromResult(Result.Failure<CalculateSaleResponse>(SaleErrors.CantidadInvalida));

            if (command.Items.Any(i => i.PrecioUnitario < 0))
                return Task.FromResult(Result.Failure<CalculateSaleResponse>(SaleErrors.PrecioInvalido));

            var response = _calculationService.Calculate(command);

            if (command.DescuentoGlobal > response.ValorAfecto)
                return Task.FromResult(Result.Failure<CalculateSaleResponse>(SaleErrors.DescuentoExcedeTotalAfecto));

            return Task.FromResult(Result.Success(response));
        }
    }
}
