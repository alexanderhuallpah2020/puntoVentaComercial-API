using DataConsulting.PuntoVentaComercial.Domain.ClasesSunat;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class ClaseSunatRepository(ApplicationDbContext dbContext)
           : Repository<ClaseSunat>(dbContext), IClaseSunatRepository
    {
        public async Task<bool> ExistsByCodigoAsync(
            int idFamiliaSunat,
            string codigo,
            CancellationToken cancellationToken = default)
        {
            string norm = codigo.Trim().ToUpper();

            return await DbContext.ClasesSunat
                .AnyAsync(x => x.IdFamiliaSunat == idFamiliaSunat && x.Codigo == norm, cancellationToken);
        }

        public async Task<int> GetNextIdAsync(CancellationToken cancellationToken = default)
        {
            int maxId = await DbContext.ClasesSunat
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0;

            return maxId + 1;
        }
    }
}
