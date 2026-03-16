using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetCurrencies
{
    public sealed record GetCurrenciesQuery() : IQuery<GetCurrenciesResponse>;
}
