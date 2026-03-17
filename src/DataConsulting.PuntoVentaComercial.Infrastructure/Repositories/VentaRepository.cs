using DataConsulting.PuntoVentaComercial.Domain.Ventas;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories;

internal sealed class VentaRepository(ApplicationDbContext dbContext)
    : Repository<Venta>(dbContext), IVentaRepository
{
    public new async Task<Venta?> GetByIdAsync(int idVenta, CancellationToken ct) =>
        await DbContext.Ventas
            .Include(x => x.Detalles)
            .Include(x => x.Pagos)
            .FirstOrDefaultAsync(x => x.Id == idVenta, ct);

    public async Task<(IList<Venta> Items, int Total)> SearchAsync(
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int? idCliente,
        short? numSerie,
        short? idTipoDocumento,
        string? estado,
        short? idSucursal,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = DbContext.Ventas.AsNoTracking();

        if (fechaDesde.HasValue)
            query = query.Where(x => x.FechaEmision >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(x => x.FechaEmision <= fechaHasta.Value);

        if (idCliente.HasValue)
            query = query.Where(x => x.IdCliente == idCliente.Value);

        if (numSerie.HasValue)
            query = query.Where(x => x.NumSerie == numSerie.Value);

        if (idTipoDocumento.HasValue)
            query = query.Where(x => x.IdTipoDocumento == idTipoDocumento.Value);

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(x => x.Estado == estado);

        if (idSucursal.HasValue)
            query = query.Where(x => x.IdSucursal == idSucursal.Value);

        int total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(x => x.FechaEmision)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<int> GetNextNumeroDocumentoAsync(
        short idSucursal, short idTipoDocumento, short numSerie, CancellationToken ct)
    {
        // Llama al SP atómico que incrementa CorrelativoDocumento.UltimoDocumento
        var result = await DbContext.Database
            .SqlQuery<int>($"EXEC dbo.GetNuevoCorrelativoDocumento {idSucursal}, {idTipoDocumento}, {numSerie}, N''")
            .ToListAsync(ct);
        return result.FirstOrDefault();
    }
}
