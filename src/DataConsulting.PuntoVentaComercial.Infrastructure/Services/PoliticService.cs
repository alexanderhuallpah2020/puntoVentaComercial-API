using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Services;

internal sealed class PoliticService(ApplicationDbContext dbContext) : IPoliticService
{
    public async Task<bool> HasPoliticAsync(
        string userName, EPolitica politic, CancellationToken cancellationToken)
    {
        var conn = dbContext.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(cancellationToken);

        using DbCommand cmd = conn.CreateCommand();
        cmd.CommandText = "Security.GetGrantedPolitic";
        cmd.CommandType = CommandType.StoredProcedure;

        var pUser = cmd.CreateParameter();
        pUser.ParameterName = "@UserName";
        pUser.DbType = DbType.AnsiString;
        pUser.Size = 20;
        pUser.Value = userName;
        cmd.Parameters.Add(pUser);

        var pPolitic = cmd.CreateParameter();
        pPolitic.ParameterName = "@PoliticId";
        pPolitic.DbType = DbType.Int32;
        pPolitic.Value = (int)politic;
        cmd.Parameters.Add(pPolitic);

        using DbDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
        if (await reader.ReadAsync(cancellationToken))
        {
            // Columna Usuario != null → política otorgada
            int usuarioOrdinal = reader.GetOrdinal("Usuario");
            return !reader.IsDBNull(usuarioOrdinal);
        }

        return false;
    }
}
