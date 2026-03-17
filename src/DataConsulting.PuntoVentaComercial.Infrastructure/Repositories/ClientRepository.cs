using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Clients;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class ClientRepository(
        ApplicationDbContext dbContext,
        IDbConnectionFactory connectionFactory)
        : Repository<Client>(dbContext), IClientRepository
    {
        /// <summary>
        /// Obtiene el cliente incluyendo sus locales (direcciones), necesario para UpdateLocal.
        /// Sobreescribe el GetByIdAsync del base para incluir la navegación privada _locals.
        /// </summary>
        public new async Task<Client?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<Client>()
                .Include("_locals")
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<ClientSearchResult>> SearchAsync(
            string? nombre,
            string? numDocumento,
            int? idDocumentoIdentidad,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT TOP (@PageSize)
                    c.IdCliente,
                    c.Nombre,
                    c.NombreComercial,
                    c.IdDocumentoIdentidad,
                    c.NumDocumento,
                    c.EstadoCliente,
                    ISNULL(lc.DireccionLocal, '') AS DireccionLocal,
                    ISNULL(lc.Telefono1, '')      AS Telefono1
                FROM Cliente c
                LEFT JOIN LocalCliente lc ON lc.IdCliente = c.IdCliente
                WHERE c.EstadoCliente = 'A'
                    AND (@Nombre         IS NULL OR c.Nombre      LIKE '%' + @Nombre + '%')
                    AND (@NumDocumento   IS NULL OR c.NumDocumento = @NumDocumento)
                    AND (@IdDocIdentidad IS NULL OR c.IdDocumentoIdentidad = @IdDocIdentidad)
                ORDER BY c.Nombre
                """;

            var result = await connection.QueryAsync<ClientSearchResult>(sql, new
            {
                PageSize = pageSize,
                Nombre = string.IsNullOrWhiteSpace(nombre) ? null : nombre.Trim(),
                NumDocumento = string.IsNullOrWhiteSpace(numDocumento) ? null : numDocumento.Trim(),
                IdDocIdentidad = idDocumentoIdentidad
            });

            return result.ToList();
        }

        public async Task<IReadOnlyList<ClientLocal>> GetLocalsAsync(
            int idCliente,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT
                    IdLocal        AS Id,
                    IdCliente,
                    IdSucursal,
                    DireccionLocal,
                    Telefono1,
                    IdTipoCliente,
                    Estado
                FROM LocalCliente
                WHERE IdCliente = @IdCliente AND Estado = 'A'
                ORDER BY IdLocal
                """;

            var rows = await connection.QueryAsync<ClientLocalRow>(sql, new { IdCliente = idCliente });

            return rows.Select(r => ClientLocal.Reconstruct(
                r.Id,
                r.IdCliente,
                r.IdSucursal,
                r.DireccionLocal,
                r.Telefono1,
                r.IdTipoCliente,
                r.Estado)).ToList();
        }

        public async Task<bool> ExistsByDocumentoAsync(
            int idDocumentoIdentidad,
            string numDocumento,
            int? excludeIdCliente,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT CASE WHEN EXISTS (
                    SELECT 1 FROM Cliente
                    WHERE IdDocumentoIdentidad = @IdDocumentoIdentidad
                      AND NumDocumento = @NumDocumento
                      AND EstadoCliente = 'A'
                      AND (@ExcludeId IS NULL OR IdCliente <> @ExcludeId)
                ) THEN 1 ELSE 0 END
                """;

            var exists = await connection.ExecuteScalarAsync<int>(sql, new
            {
                IdDocumentoIdentidad = idDocumentoIdentidad,
                NumDocumento = numDocumento,
                ExcludeId = excludeIdCliente
            });

            return exists == 1;
        }

        public void Update(Client client)
        {
            DbContext.Update(client);
        }

        private sealed record ClientLocalRow(
            int Id,
            int IdCliente,
            int IdSucursal,
            string DireccionLocal,
            string Telefono1,
            int IdTipoCliente,
            string Estado);
    }
}
