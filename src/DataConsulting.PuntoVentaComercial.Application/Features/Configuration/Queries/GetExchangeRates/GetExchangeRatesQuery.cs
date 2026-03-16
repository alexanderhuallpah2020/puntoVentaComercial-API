using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetExchangeRates
{
    public sealed record GetExchangeRatesQuery(
        int IdEmpresa,
        DateOnly? Fecha
    ) : IQuery<GetExchangeRatesResponse>;
}
