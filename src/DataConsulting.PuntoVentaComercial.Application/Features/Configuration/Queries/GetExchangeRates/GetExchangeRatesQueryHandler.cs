using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Configuration;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetExchangeRates
{
    internal sealed class GetExchangeRatesQueryHandler(IExchangeRateRepository repository)
        : IQueryHandler<GetExchangeRatesQuery, GetExchangeRatesResponse>
    {
        public async Task<Result<GetExchangeRatesResponse>> Handle(
            GetExchangeRatesQuery query,
            CancellationToken cancellationToken)
        {
            var fecha = query.Fecha ?? DateOnly.FromDateTime(DateTime.Today);

            var rates = await repository.GetByFechaAsync(
                query.IdEmpresa, fecha, cancellationToken);

            if (rates.Count == 0)
                return Result.Failure<GetExchangeRatesResponse>(ConfigurationErrors.SinTipoCambio);

            var dtos = rates.Select(r => new ExchangeRateDto(
                r.Id,
                r.IdEmpresa,
                r.Fecha.ToString("yyyy-MM-dd"),
                (int)r.TipoMoneda,
                r.TipoMoneda.ToString(),
                r.TipoCambioCompra,
                r.TipoCambioVenta
            )).ToList();

            return Result.Success(new GetExchangeRatesResponse(dtos));
        }
    }
}
