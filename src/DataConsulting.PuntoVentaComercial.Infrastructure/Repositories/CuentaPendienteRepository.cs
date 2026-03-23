using DataConsulting.PuntoVentaComercial.Domain.CuentasPendientes;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories;

internal sealed class CuentaPendienteRepository(ApplicationDbContext dbContext)
    : ICuentaPendienteRepository
{
    public void Add(CuentaPendiente cuentaPendiente) =>
        dbContext.CuentasPendientes.Add(cuentaPendiente);

    public async Task<CuentaPendiente?> GetByVentaAsync(
        short idEmpresa, int idVenta, short secuencia, CancellationToken ct) =>
        await dbContext.CuentasPendientes
            .FirstOrDefaultAsync(x =>
                x.IdEmpresa     == idEmpresa  &&
                x.TipoOperacion == 2          &&
                x.IdOperacion   == idVenta    &&
                x.Secuencia     == secuencia, ct);
}
