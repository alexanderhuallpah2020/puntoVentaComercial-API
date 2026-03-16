using DataConsulting.PuntoVentaComercial.Domain.Configuration;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class ShiftRepository(ApplicationDbContext dbContext) : IShiftRepository
    {
        public async Task<IReadOnlyList<Shift>> GetActiveShiftsAsync(
            int idEmpresa,
            TimeOnly horaActual,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<Shift>()
                .Where(s => s.IdEmpresa == idEmpresa
                         && s.Activo
                         && s.HoraInicio <= horaActual
                         && s.HoraFin >= horaActual)
                .OrderBy(s => s.HoraInicio)
                .ToListAsync(cancellationToken);
        }
    }
}
