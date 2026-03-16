using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Configuration;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class SystemConstantRepository(IDbConnectionFactory connectionFactory) : ISystemConstantRepository
    {
        public async Task<IReadOnlyList<SystemConstant>> GetConstantsAsync(
            int idEmpresa,
            int idSucursal,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT Clave, Valor, Descripcion
                FROM Constante
                WHERE (IdEmpresa = @IdEmpresa OR IdEmpresa IS NULL)
                  AND (IdSucursal = @IdSucursal OR IdSucursal IS NULL)
                  AND Estado = 1
                ORDER BY Clave
                """;

            var result = await connection.QueryAsync<SystemConstant>(sql, new
            {
                IdEmpresa = idEmpresa,
                IdSucursal = idSucursal
            });

            return result.ToList();
        }
    }
}
