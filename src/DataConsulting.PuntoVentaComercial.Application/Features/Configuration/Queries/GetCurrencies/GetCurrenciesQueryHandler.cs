using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Configuration;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetCurrencies
{
    internal sealed class GetCurrenciesQueryHandler(IExchangeRateRepository repository)
        : IQueryHandler<GetCurrenciesQuery, GetCurrenciesResponse>
    {
        public async Task<Result<GetCurrenciesResponse>> Handle(
            GetCurrenciesQuery query,
            CancellationToken cancellationToken)
        {
            var currencies = await repository.GetActiveCurrenciesAsync(cancellationToken);

            var dtos = currencies.Select(c => new CurrencyDto(
                c.Id,
                (int)c.TipoMoneda,
                c.Codigo,
                c.Simbolo,
                c.Descripcion
            )).ToList();

            return Result.Success(new GetCurrenciesResponse(dtos));
        }
    }
}
