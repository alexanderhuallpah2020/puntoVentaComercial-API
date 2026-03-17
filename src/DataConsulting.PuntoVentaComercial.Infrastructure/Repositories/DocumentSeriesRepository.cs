using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Configuration;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class DocumentSeriesRepository(
        IDbConnectionFactory connectionFactory,
        ApplicationDbContext dbContext) : IDocumentSeriesRepository
    {
        public async Task<IReadOnlyList<DocumentSeries>> GetByEstacionAsync(
            int idEmpresa,
            int idSucursal,
            int idEstacion,
            CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<DocumentSeries>()
                .Where(s => s.IdEmpresa == idEmpresa
                         && s.IdSucursal == idSucursal
                         && s.IdEstacion == idEstacion
                         && s.Activo)
                .OrderBy(s => s.TipoDocumento)
                .ThenBy(s => s.NumSerie)
                .ToListAsync(cancellationToken);
        }

        public async Task<long> GetNextCorrelativeAsync(
            int idEmpresa,
            int idSucursal,
            EDocumento tipoDocumento,
            string numSerie,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT ISNULL(UltimoCorrelativo, 0) + 1
                FROM DocumentoSerie
                WHERE IdEmpresa = @IdEmpresa
                  AND IdSucursal = @IdSucursal
                  AND IdTipoDocumento = @TipoDocumento
                  AND NumSerie = @NumSerie
                  AND Estado = 1
                """;

            return await connection.ExecuteScalarAsync<long>(sql, new
            {
                IdEmpresa = idEmpresa,
                IdSucursal = idSucursal,
                TipoDocumento = (int)tipoDocumento,
                NumSerie = numSerie
            });
        }

        public async Task<long> IncrementAndGetCorrelativeAsync(
            int idEmpresa,
            int idSucursal,
            EDocumento tipoDocumento,
            string numSerie,
            CancellationToken cancellationToken = default)
        {
            // UPDATE atómico: incrementa y retorna el nuevo correlativo.
            // Si hay concurrencia, el UPDATE serializa las peticiones a nivel de fila.
            var sql = """
                UPDATE DocumentoSerie
                SET UltimoCorrelativo = UltimoCorrelativo + 1
                OUTPUT INSERTED.UltimoCorrelativo
                WHERE IdEmpresa        = @IdEmpresa
                  AND IdSucursal       = @IdSucursal
                  AND IdTipoDocumento  = @TipoDocumento
                  AND NumSerie         = @NumSerie
                  AND Estado           = 1
                """;

            await using var connection = await connectionFactory.OpenConnectionAsync();

            var newCorrelativo = await connection.ExecuteScalarAsync<long?>(sql, new
            {
                IdEmpresa = idEmpresa,
                IdSucursal = idSucursal,
                TipoDocumento = (int)tipoDocumento,
                NumSerie = numSerie
            });

            return newCorrelativo ?? 0;
        }
    }
}
