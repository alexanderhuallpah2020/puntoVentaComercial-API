using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Identity;
using System.Data.Common;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Auth.Queries.GetSessionEnvironment
{
    internal sealed class GetSessionEnvironmentQueryHandler(IDbConnectionFactory dbConnectionFactory)
        : IQueryHandler<GetSessionEnvironmentQuery, GetSessionEnvironmentResponse>
    {
        public async Task<Result<GetSessionEnvironmentResponse>> Handle(
            GetSessionEnvironmentQuery query,
            CancellationToken cancellationToken)
        {
            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

            // TODO: Verificar nombres de tablas y columnas contra el esquema real del legacy.
            // Las tablas sugeridas son: Empresa, Sucursal, Estacion, Trabajador, EmpresaConstante.
            const string sql =
                $"""
                SELECT
                    e.IdEmpresa            AS {nameof(GetSessionEnvironmentResponse.IdEmpresa)},
                    e.Nombre               AS {nameof(GetSessionEnvironmentResponse.NombreEmpresa)},
                    e.Ruc                  AS {nameof(GetSessionEnvironmentResponse.RucEmpresa)},
                    s.IdSucursal           AS {nameof(GetSessionEnvironmentResponse.IdSucursal)},
                    s.Nombre               AS {nameof(GetSessionEnvironmentResponse.NombreSucursal)},
                    est.IdEstacion         AS {nameof(GetSessionEnvironmentResponse.IdEstacion)},
                    est.Codigo             AS {nameof(GetSessionEnvironmentResponse.CodigoEstacion)},
                    t.IdTrabajador         AS {nameof(GetSessionEnvironmentResponse.IdTrabajador)},
                    t.Nombre               AS {nameof(GetSessionEnvironmentResponse.NombreTrabajador)},
                    ISNULL(c.MontoMaximoBoleta, 700)  AS {nameof(GetSessionEnvironmentResponse.MontoMaximoBoleta)},
                    ISNULL(c.UsaIgv, 1)    AS {nameof(GetSessionEnvironmentResponse.UsaIgv)},
                    ISNULL(c.UsaIsc, 0)    AS {nameof(GetSessionEnvironmentResponse.UsaIsc)}
                FROM       dbo.Empresa      e
                INNER JOIN dbo.Sucursal     s   ON  s.IdEmpresa   = e.IdEmpresa
                INNER JOIN dbo.Estacion     est ON  est.IdSucursal = s.IdSucursal
                INNER JOIN dbo.Trabajador   t   ON  t.IdTrabajador = @IdTrabajador
                LEFT  JOIN dbo.EmpresaConstante c ON c.IdEmpresa  = e.IdEmpresa
                WHERE e.IdEmpresa   = @IdEmpresa
                  AND s.IdSucursal  = @IdSucursal
                  AND est.IdEstacion = @IdEstacion;
                """;

            var command = new CommandDefinition(
                sql,
                new
                {
                    query.IdEmpresa,
                    query.IdSucursal,
                    query.IdEstacion,
                    query.IdTrabajador
                },
                cancellationToken: cancellationToken);

            var result = await connection.QuerySingleOrDefaultAsync<GetSessionEnvironmentResponse>(command);

            if (result is null)
                return Result.Failure<GetSessionEnvironmentResponse>(IdentityErrors.TokenInvalido);

            return Result.Success(result);
        }
    }
}
