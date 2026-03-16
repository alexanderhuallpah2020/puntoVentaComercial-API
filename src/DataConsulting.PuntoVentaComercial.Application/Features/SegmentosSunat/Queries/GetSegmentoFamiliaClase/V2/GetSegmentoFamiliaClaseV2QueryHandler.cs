using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase.V2
{
    internal sealed class GetSegmentoFamiliaClaseV2QueryHandler(IDbConnectionFactory dbConnectionFactory)
          : IQueryHandler<GetSegmentoFamiliaClaseV2Query, List<GetSegmentoFamiliaClaseResponse>>
    {
        public async Task<Result<List<GetSegmentoFamiliaClaseResponse>>> Handle(
            GetSegmentoFamiliaClaseV2Query query,
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

            var command = new CommandDefinition(sql, cancellationToken: cancellationToken);

            var rows = await connection.QueryAsync<GetSegmentoFamiliaClaseResponse>(command);

            return rows.AsList();
        }
    }
}
