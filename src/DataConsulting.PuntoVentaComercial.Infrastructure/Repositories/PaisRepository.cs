using DataConsulting.PuntoVentaComercial.Domain.Paises;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories;

internal sealed class PaisRepository(ApplicationDbContext dbContext)
    : Repository<Pais>(dbContext), IPaisRepository
{
    public async Task<bool> ExistsAsync(short id, CancellationToken ct) =>
        await DbContext.Paises.AnyAsync(x => x.Id == id, ct);
}
