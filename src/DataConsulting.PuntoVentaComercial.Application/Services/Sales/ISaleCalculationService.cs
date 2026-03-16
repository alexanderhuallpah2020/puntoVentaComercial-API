using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CalculateSale;

namespace DataConsulting.PuntoVentaComercial.Application.Services.Sales
{
    public interface ISaleCalculationService
    {
        CalculateSaleResponse Calculate(CalculateSaleCommand command);
    }
}
