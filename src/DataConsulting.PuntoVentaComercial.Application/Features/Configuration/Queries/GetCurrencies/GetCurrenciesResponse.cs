namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetCurrencies
{
    public sealed record GetCurrenciesResponse(IReadOnlyList<CurrencyDto> Currencies);

    public sealed record CurrencyDto(
        int Id,
        int TipoMoneda,
        string Codigo,
        string Simbolo,
        string Descripcion
    );
}
