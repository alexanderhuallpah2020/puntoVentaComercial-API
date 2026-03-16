using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Identity;
using System.Data.Common;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    // TODO: Verificar nombres de tabla y columnas contra el esquema real del legacy.
    internal sealed class WorkstationRepository(IDbConnectionFactory dbConnectionFactory) : IWorkstationRepository
    {
        public async Task<Workstation?> GetByCodigoAndEmpresaAsync(
            string codigo,
            int idEmpresa,
            CancellationToken cancellationToken = default)
        {
            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

            const string sql =
                """
                SELECT
                    e.IdEstacion,
                    e.Codigo,
                    e.IdSucursal,
                    s.Nombre  AS NombreSucursal,
                    e.IdEmpresa,
                    e.Estado  AS Activo
                FROM dbo.Estacion  e
                INNER JOIN dbo.Sucursal s ON s.IdSucursal = e.IdSucursal
                WHERE e.Codigo    = @Codigo
                  AND e.IdEmpresa = @IdEmpresa;
                """;

            var command = new CommandDefinition(
                sql,
                new { Codigo = codigo, IdEmpresa = idEmpresa },
                cancellationToken: cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<Workstation>(command);
        }
    }
}
