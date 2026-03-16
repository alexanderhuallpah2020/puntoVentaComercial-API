using DataConsulting.PuntoVentaComercial.Domain.Configuration;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class SellerRepository(ApplicationDbContext dbContext) : ISellerRepository
    {
        public async Task<IReadOnlyList<Seller>> GetActiveSellersAsync(
            int idEmpresa,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<Seller>()
                .Where(s => s.IdEmpresa == idEmpresa && s.Activo)
                .OrderBy(s => s.Apellidos)
                .ThenBy(s => s.Nombres)
                .ToListAsync(cancellationToken);
        }
    }
}
