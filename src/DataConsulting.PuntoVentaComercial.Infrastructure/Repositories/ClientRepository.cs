using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Clients;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using System.Data.Common;
using System.Text;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    // TODO: Verificar nombres de tabla y columnas contra el esquema real del legacy.
    // Tablas esperadas: dbo.Cliente, dbo.LocalCliente
    internal sealed class ClientRepository(IDbConnectionFactory dbConnectionFactory) : IClientRepository
    {
        public async Task<Client?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

            const string sql =
                """
                SELECT
                    c.IdCliente         AS Id,
                    c.Nombre,
                    c.NombreComercial,
                    c.IdDocumentoIdentidad,
                    c.NumDocumento,
                    c.CodValidadorDoc,
                    c.IdPais,
                    c.Direccion,
                    c.FlagIGV,
                    c.CreditoMaximo,
                    c.EstadoCliente,
                    c.FechaAlta,
                    c.FechaBaja,
                    c.IdUsuarioCreador,
                    c.FechaCreacion,
                    c.IdUsuarioModificador,
                    c.FechaModificacion
                FROM dbo.Cliente c
                WHERE c.IdCliente = @Id;
                """;

            var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: ct);
            return await connection.QuerySingleOrDefaultAsync<Client>(command);
        }

        public async Task<List<Client>> SearchAsync(
            string? nombre,
            string? numDocumento,
            EDocumentoIdentidad? tipoDocumento,
            int idEmpresa,
            CancellationToken ct = default)
        {
            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

            var sql = new StringBuilder(
                """
                SELECT
                    c.IdCliente         AS Id,
                    c.Nombre,
                    c.NombreComercial,
                    c.IdDocumentoIdentidad,
                    c.NumDocumento,
                    c.CodValidadorDoc,
                    c.IdPais,
                    c.Direccion,
                    c.FlagIGV,
                    c.CreditoMaximo,
                    c.EstadoCliente,
                    c.FechaAlta,
                    c.FechaBaja,
                    c.IdUsuarioCreador,
                    c.FechaCreacion,
                    c.IdUsuarioModificador,
                    c.FechaModificacion
                FROM dbo.Cliente c
                WHERE c.IdEmpresa = @IdEmpresa
                """);

            if (!string.IsNullOrWhiteSpace(nombre))
                sql.Append(" AND c.Nombre LIKE @Nombre");

            if (!string.IsNullOrWhiteSpace(numDocumento))
                sql.Append(" AND c.NumDocumento = @NumDocumento");

            if (tipoDocumento.HasValue && tipoDocumento.Value != EDocumentoIdentidad.Todos)
                sql.Append(" AND c.IdDocumentoIdentidad = @TipoDocumento");

            sql.Append(" ORDER BY c.Nombre");

            var command = new CommandDefinition(
                sql.ToString(),
                new
                {
                    IdEmpresa = idEmpresa,
                    Nombre = $"%{nombre}%",
                    NumDocumento = numDocumento,
                    TipoDocumento = (int?)tipoDocumento
                },
                cancellationToken: ct);

            var result = await connection.QueryAsync<Client>(command);
            return result.AsList();
        }

        public async Task<List<ClientLocal>> GetAddressesAsync(int idCliente, CancellationToken ct = default)
        {
            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

            const string sql =
                """
                SELECT
                    lc.IdLocal          AS Id,
                    lc.IdCliente,
                    lc.DireccionLocal,
                    lc.Telefono1,
                    lc.IdTipoCliente,
                    lc.IdSucursal,
                    lc.IdRuta,
                    lc.Estado
                FROM dbo.LocalCliente lc
                WHERE lc.IdCliente = @IdCliente
                  AND lc.Estado    = 1
                ORDER BY lc.IdLocal;
                """;

            var command = new CommandDefinition(sql, new { IdCliente = idCliente }, cancellationToken: ct);
            var result = await connection.QueryAsync<ClientLocal>(command);
            return result.AsList();
        }

        public async Task<bool> ExistsByDocumentoAsync(
            EDocumentoIdentidad tipo,
            string numero,
            CancellationToken ct = default)
        {
            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

            const string sql =
                """
                SELECT COUNT(1)
                FROM dbo.Cliente
                WHERE IdDocumentoIdentidad = @Tipo
                  AND NumDocumento         = @Numero;
                """;

            var command = new CommandDefinition(
                sql,
                new { Tipo = (int)tipo, Numero = numero },
                cancellationToken: ct);

            int count = await connection.ExecuteScalarAsync<int>(command);
            return count > 0;
        }

        public async Task<int> InsertAsync(
            Client client,
            string telefono1,
            int idSucursal,
            CancellationToken ct = default)
        {
            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

            // TODO: Ajustar a columnas reales del esquema legacy (IdEmpresa puede requerirse).
            const string sql =
                """
                INSERT INTO dbo.Cliente (
                    Nombre, NombreComercial, IdDocumentoIdentidad, NumDocumento,
                    CodValidadorDoc, IdPais, Direccion, FlagIGV, CreditoMaximo,
                    EstadoCliente, FechaAlta, IdUsuarioCreador, FechaCreacion)
                OUTPUT INSERTED.IdCliente
                VALUES (
                    @Nombre, @NombreComercial, @IdDocumentoIdentidad, @NumDocumento,
                    @CodValidadorDoc, @IdPais, @Direccion, @FlagIGV, @CreditoMaximo,
                    @EstadoCliente, @FechaAlta, @IdUsuarioCreador, @FechaCreacion);

                -- Insertar dirección principal
                INSERT INTO dbo.LocalCliente (
                    IdCliente, DireccionLocal, Telefono1, IdTipoCliente, IdSucursal, Estado)
                VALUES (
                    SCOPE_IDENTITY(), @Direccion, @Telefono1, 1, @IdSucursal, 1);
                """;

            var command = new CommandDefinition(
                sql,
                new
                {
                    client.Nombre,
                    client.NombreComercial,
                    IdDocumentoIdentidad = (int)client.IdDocumentoIdentidad,
                    client.NumDocumento,
                    client.CodValidadorDoc,
                    client.IdPais,
                    client.Direccion,
                    client.FlagIGV,
                    client.CreditoMaximo,
                    EstadoCliente = (int)client.EstadoCliente,
                    client.FechaAlta,
                    client.IdUsuarioCreador,
                    client.FechaCreacion,
                    Telefono1 = telefono1,
                    IdSucursal = idSucursal
                },
                cancellationToken: ct);

            return await connection.ExecuteScalarAsync<int>(command);
        }

        public async Task UpdateAsync(Client client, CancellationToken ct = default)
        {
            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

            const string sql =
                """
                UPDATE dbo.Cliente SET
                    Nombre               = @Nombre,
                    NombreComercial      = @NombreComercial,
                    Direccion            = @Direccion,
                    FlagIGV              = @FlagIGV,
                    CreditoMaximo        = @CreditoMaximo,
                    IdUsuarioModificador = @IdUsuarioModificador,
                    FechaModificacion    = @FechaModificacion
                WHERE IdCliente = @IdCliente;
                """;

            var command = new CommandDefinition(
                sql,
                new
                {
                    client.Nombre,
                    client.NombreComercial,
                    client.Direccion,
                    client.FlagIGV,
                    client.CreditoMaximo,
                    client.IdUsuarioModificador,
                    client.FechaModificacion,
                    IdCliente = client.IdCliente
                },
                cancellationToken: ct);

            await connection.ExecuteAsync(command);
        }
    }
}
