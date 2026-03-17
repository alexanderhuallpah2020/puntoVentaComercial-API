using DataConsulting.PuntoVentaComercial.Domain.DocumentosIdentidad;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories;

internal sealed class DocumentoIdentidadRepository(ApplicationDbContext dbContext)
    : Repository<DocumentoIdentidad>(dbContext), IDocumentoIdentidadRepository
{
    public async Task<bool> ExistsAsync(int id, CancellationToken ct) =>
        await DbContext.DocumentosIdentidad.AnyAsync(x => x.Id == id, ct);
}
