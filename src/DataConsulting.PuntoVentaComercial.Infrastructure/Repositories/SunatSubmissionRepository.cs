using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using DataConsulting.PuntoVentaComercial.Domain.Sunat;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class SunatSubmissionRepository(
        ApplicationDbContext dbContext,
        IDbConnectionFactory connectionFactory)
        : Repository<SunatSubmission>(dbContext), ISunatSubmissionRepository
    {
        public async Task<SunatSubmission?> GetByIdVentaAsync(
            int idVenta,
            CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<SunatSubmission>()
                .FirstOrDefaultAsync(s => s.IdVenta == idVenta, cancellationToken);
        }

        public async Task<IReadOnlyList<PendingSubmissionResult>> GetPendingAsync(
            int idEmpresa,
            int idSucursal,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            // Retorna ventas electrónicas que no han sido aceptadas (pendientes + rechazadas)
            // o que nunca han tenido un intento de envío (LEFT JOIN con SunatEnvio)
            var sql = """
                SELECT TOP (@PageSize)
                    v.IdVenta,
                    v.IdEmpresa,
                    v.IdSucursal,
                    v.FechaEmision,
                    v.IdTipoDocumento  AS TipoDocumento,
                    v.NumSerie,
                    v.Correlativo,
                    v.NombreCliente,
                    v.NumDocumentoCliente,
                    v.ImporteTotal,
                    s.Estado           AS EstadoSunat,
                    s.CodigoRespuesta,
                    s.MensajeRespuesta,
                    ISNULL(s.Intentos, 0) AS Intentos
                FROM Venta v
                LEFT JOIN SunatEnvio s ON s.IdVenta = v.IdVenta
                WHERE v.IdEmpresa   = @IdEmpresa
                  AND v.IdSucursal  = @IdSucursal
                  AND v.Estado      = 1
                  AND v.IdTipoDocumento IN (184, 191, 186, 187) -- solo comprobantes electrónicos
                  AND (s.Estado IS NULL OR s.Estado <> @EstadoAceptado)
                  AND (@FechaDesde IS NULL OR v.FechaEmision >= @FechaDesde)
                  AND (@FechaHasta IS NULL OR v.FechaEmision <= @FechaHasta)
                ORDER BY v.FechaEmision ASC, v.IdVenta ASC
                """;

            var result = await connection.QueryAsync<PendingSubmissionResult>(sql, new
            {
                PageSize = pageSize,
                IdEmpresa = idEmpresa,
                IdSucursal = idSucursal,
                EstadoAceptado = (int)ETipoEstadoSunat.Aceptado,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta
            });

            return result.ToList();
        }

        public void Update(SunatSubmission submission)
        {
            DbContext.Update(submission);
        }
    }
}
