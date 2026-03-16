using DataConsulting.PuntoVentaComercial.Domain.SegmentosSunat;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class SegmentoSunatRepository(ApplicationDbContext dbContext)
    : Repository<SegmentoSunat>(dbContext), ISegmentoSunatRepository
    {
        public async Task<IReadOnlyList<SegmentoSunat>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await DbContext.SegmentosSunat
                .AsNoTracking()
                .OrderBy(x => x.Descripcion)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodigoAsync(
        string codigo,
        CancellationToken cancellationToken = default)
        {
            return await DbContext.SegmentosSunat
                .AnyAsync(x => x.Codigo == codigo, cancellationToken);
        }

        public async Task<int> GetNextIdAsync(CancellationToken cancellationToken = default)
        {
            int maxId = await DbContext.SegmentosSunat
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0;

            return maxId + 1;
        }
    }
}
