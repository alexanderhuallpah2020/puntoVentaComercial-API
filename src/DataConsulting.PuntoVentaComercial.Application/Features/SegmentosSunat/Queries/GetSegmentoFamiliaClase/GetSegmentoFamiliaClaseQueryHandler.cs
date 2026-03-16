using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using System.Data.Common;

namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase
{
    internal sealed class GetSegmentoFamiliaClaseQueryHandler(IDbConnectionFactory dbConnectionFactory)
         : IQueryHandler<GetSegmentoFamiliaClaseQuery, List<GetSegmentoFamiliaClaseResponse>>
    {
        public async Task<Result<List<GetSegmentoFamiliaClaseResponse>>> Handle(
            GetSegmentoFamiliaClaseQuery request,
            CancellationToken cancellationToken)
        {
            await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

           const string sql =
                $"""
                SELECT
                    s.Codigo      AS {nameof(GetSegmentoFamiliaClaseResponse.Segmento)},
                    s.Descripcion AS {nameof(GetSegmentoFamiliaClaseResponse.SegmentoDescripcion)},
                    f.Codigo      AS {nameof(GetSegmentoFamiliaClaseResponse.Familia)},
                    f.Descripcion AS {nameof(GetSegmentoFamiliaClaseResponse.FamiliaDescripcion)},
                    c.Codigo      AS {nameof(GetSegmentoFamiliaClaseResponse.Clase)},
                    c.Descripcion AS {nameof(GetSegmentoFamiliaClaseResponse.ClaseDescripcion)}
                FROM dbo.SegmentoSunat s
                INNER JOIN dbo.FamiliaSunat f ON f.IdSegmentoSunat = s.IdSegmentoSunat
                INNER JOIN dbo.ClaseSunat c ON c.IdFamiliaSunat = f.IdFamiliaSunat
                ORDER BY s.Codigo, f.Codigo, c.Codigo;
                """;

            var rows = await connection.QueryAsync<GetSegmentoFamiliaClaseResponse>(sql);

            return rows.AsList();
        }
    }
}
