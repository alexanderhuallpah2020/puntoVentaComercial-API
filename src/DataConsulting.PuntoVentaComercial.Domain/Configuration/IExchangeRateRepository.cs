namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    public interface IExchangeRateRepository
    {
        Task<IReadOnlyList<ExchangeRate>> GetByFechaAsync(
            int idEmpresa,
            DateOnly fecha,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Currency>> GetActiveCurrenciesAsync(
            CancellationToken cancellationToken = default);
    }
}
