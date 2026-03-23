using DataConsulting.PuntoVentaComercial.Domain.OperacionesPago;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories;

internal sealed class OperacionPagoRepository(ApplicationDbContext dbContext)
    : IOperacionPagoRepository
{
    public void Add(OperacionPago operacionPago) =>
        dbContext.OperacionesPago.Add(operacionPago);

    public async Task<int> GetNextNroOperacionAsync(
        short idEmpresa, byte tipoOperacion, CancellationToken ct)
    {
        int max = await dbContext.OperacionesPago
            .AsNoTracking()
            .Where(x => x.IdEmpresa == idEmpresa && x.TipoOperacion == tipoOperacion)
            .MaxAsync(x => (int?)x.NroOperacion, ct) ?? 0;

        return max + 1;
    }
}
