using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories;

internal sealed class ClienteRepository(ApplicationDbContext dbContext)
    : Repository<Cliente>(dbContext), IClienteRepository
{
    public new async Task<Cliente?> GetByIdAsync(int idCliente, CancellationToken ct) =>
        await DbContext.Clientes
            .Include(x => x.ClienteLocales)
            .FirstOrDefaultAsync(x => x.Id == idCliente, ct);

    public async Task<bool> ExistsByDocumentoAsync(
        int idDocIdentidad, string numDocumento, CancellationToken ct) =>
        await DbContext.Clientes.AnyAsync(
            x => x.IdDocumentoIdentidad == idDocIdentidad &&
                 x.NumDocumento == numDocumento, ct);

    public async Task<(IList<Cliente> Items, int Total)> SearchAsync(
        string? nombre, string? numDocumento,
        short? idPais, int? idDocIdentidad,
        int page, int pageSize, CancellationToken ct)
    {
        var query = DbContext.Clientes
            .Include(x => x.ClienteLocales)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(x => x.Nombre.Contains(nombre));

        if (!string.IsNullOrWhiteSpace(numDocumento))
            query = query.Where(x => x.NumDocumento == numDocumento);

        if (idPais.HasValue)
            query = query.Where(x => x.IdPais == idPais.Value);

        if (idDocIdentidad.HasValue)
            query = query.Where(x => x.IdDocumentoIdentidad == idDocIdentidad.Value);

        int total = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IList<ClienteLocal>> GetAddressesByClienteIdAsync(
        int idCliente, CancellationToken ct) =>
        await DbContext.ClienteLocales
            .AsNoTracking()
            .Where(x => x.IdCliente == idCliente)
            .ToListAsync(ct);

    public async Task<int> GetNextIdAsync(CancellationToken ct)
    {
        int max = await DbContext.Clientes.MaxAsync(x => (int?)x.Id, ct) ?? 0;
        return max + 1;
    }

    public async Task<int> GetNextLocalIdAsync(CancellationToken ct)
    {
        int max = await DbContext.ClienteLocales.MaxAsync(x => (int?)x.Id, ct) ?? 0;
        return max + 1;
    }

    public async Task<int> GetNextLocalUnicoIdAsync(CancellationToken ct)
    {
        int max = await DbContext.ClienteLocales.MaxAsync(x => (int?)x.IdLocalUnico, ct) ?? 0;
        return max + 1;
    }
}
