namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetExchangeRates
{
    public sealed record GetExchangeRatesResponse(IReadOnlyList<ExchangeRateDto> Rates);

    public sealed record ExchangeRateDto(
        int Id,
        int IdEmpresa,
        string Fecha,
        int TipoMoneda,
        string CodigoMoneda,
        decimal TipoCambioCompra,
        decimal TipoCambioVenta
    );
}
