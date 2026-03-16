using DataConsulting.PuntoVentaComercial.Domain.Configuration;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class ExchangeRateRepository(ApplicationDbContext dbContext) : IExchangeRateRepository
    {
        public async Task<IReadOnlyList<ExchangeRate>> GetByFechaAsync(
            int idEmpresa,
            DateOnly fecha,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<ExchangeRate>()
                .Where(r => r.IdEmpresa == idEmpresa && r.Fecha == fecha)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Currency>> GetActiveCurrenciesAsync(
            CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<Currency>()
                .Where(c => c.Activo)
                .OrderBy(c => c.Codigo)
                .ToListAsync(cancellationToken);
        }
    }
}
