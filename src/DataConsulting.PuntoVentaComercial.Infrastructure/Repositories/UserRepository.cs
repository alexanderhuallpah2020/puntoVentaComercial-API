using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Identity;
using System.Data.Common;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    // TODO: Verificar nombres de tabla y columnas contra el esquema real del legacy.
    internal sealed class UserRepository(IDbConnectionFactory dbConnectionFactory) : IUserRepository
    {
        public async Task<User?> GetByUsernameAndEmpresaAsync(
            string username,
            int idEmpresa,
            CancellationToken cancellationToken = default)
        {
            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

            const string sql =
                """
                SELECT
                    u.IdUsuario       AS Id,
                    u.NombreUsuario   AS Username,
                    u.Clave           AS PasswordHash,
                    u.IdEmpresa,
                    u.IdTrabajador,
                    t.Nombre          AS NombreTrabajador,
                    u.Estado          AS Activo
                FROM dbo.Usuario    u
                INNER JOIN dbo.Trabajador t ON t.IdTrabajador = u.IdTrabajador
                WHERE u.NombreUsuario = @Username
                  AND u.IdEmpresa     = @IdEmpresa;
                """;

            var command = new CommandDefinition(
                sql,
                new { Username = username, IdEmpresa = idEmpresa },
                cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<User>(command);
        }

        public async Task<List<string>> GetPoliciesAsync(
            int idUsuario,
            CancellationToken cancellationToken = default)
        {
            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

            // TODO: Ajustar tabla/columna de políticas al esquema real.
            const string sql =
                """
                SELECT p.NombrePolitica
                FROM dbo.UsuarioPolitica up
                INNER JOIN dbo.Politica p ON p.IdPolitica = up.IdPolitica
                WHERE up.IdUsuario = @IdUsuario
                  AND up.Activo    = 1;
                """;

            var command = new CommandDefinition(
                sql,
                new { IdUsuario = idUsuario },
                cancellationToken: cancellationToken);

            var result = await connection.QueryAsync<string>(command);
            return result.AsList();
        }
    }
}
