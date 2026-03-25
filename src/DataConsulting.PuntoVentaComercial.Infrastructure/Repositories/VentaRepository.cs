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

    public async Task<(IList<VentaSearchResultDto> Items, int Total)> SearchAsync(
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        string? nombreCliente,
        string? numSerieA,
        int? numDocumento,
        short? idTipoDocumento,
        string? estado,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query =
            from v in DbContext.Ventas.AsNoTracking()
            join c in DbContext.Clientes.AsNoTracking() on v.IdCliente equals c.Id
            select new { Venta = v, ClienteNombre = c.Nombre };

        if (fechaDesde.HasValue)
            query = query.Where(x => x.Venta.FechaEmision >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(x => x.Venta.FechaEmision < fechaHasta.Value.Date.AddDays(1));

        if (!string.IsNullOrWhiteSpace(nombreCliente))
            query = query.Where(x => x.ClienteNombre.Contains(nombreCliente));

        if (!string.IsNullOrWhiteSpace(numSerieA))
            query = query.Where(x => x.Venta.NumSerieA == numSerieA);

        if (numDocumento.HasValue)
        {
            string numDocumentoA = numDocumento.Value.ToString("D6");
            query = query.Where(x => x.Venta.NumeroDocumentoA == numDocumentoA);
        }

        if (idTipoDocumento.HasValue)
            query = query.Where(x => x.Venta.IdTipoDocumento == idTipoDocumento.Value);

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(x => x.Venta.Estado == estado);

        int total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.Venta.FechaEmision)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new VentaSearchResultDto(
                x.Venta.Id,
                x.Venta.IdTipoDocumento,
                x.Venta.NumSerieA,
                x.Venta.NumeroDocumentoA,
                x.ClienteNombre,
                x.Venta.Vendedor,
                x.Venta.FechaEmision,
                x.Venta.Estado,
                x.Venta.ImporteTotal))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<int> GetNroCorrelativoVentaAsync(
        DateTime fechaEmision, short idSubdiario, CancellationToken ct)
    {
        // SP devuelve MAX(NroCorrelativo) del mes/año/subdiario; el handler suma +1
        var result = await DbContext.Database
            .SqlQuery<int>($"EXEC dbo.GetNroCorrelativoVenta {fechaEmision}, {idSubdiario}")
            .ToListAsync(ct);
        return result.FirstOrDefault();
    }

    public async Task<int?> GetNextNumeroDocumentoAsync(
        short idSucursal, short idTipoDocumento, string numSerieA, CancellationToken ct)
    {
        // @NumSerie siempre es 0; el SP busca por NumSerieA (ej. 'F001', 'B001')
        // Retorna null si no existe configuración para esa serie en CorrelativoDocumento
        short numSerie = 0;
        var result = await DbContext.Database
            .SqlQuery<int?>($"EXEC dbo.GetNuevoCorrelativoDocumento {idSucursal}, {idTipoDocumento}, {numSerie}, {numSerieA}")
            .ToListAsync(ct);
        return result.FirstOrDefault();
    }
}
