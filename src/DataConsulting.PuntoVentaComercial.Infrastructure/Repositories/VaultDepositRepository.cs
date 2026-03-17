using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Cash;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class VaultDepositRepository(
        ApplicationDbContext dbContext,
        IDbConnectionFactory connectionFactory)
        : Repository<VaultDeposit>(dbContext), IVaultDepositRepository
    {
        public async Task<IReadOnlyList<VaultDepositSearchResult>> SearchAsync(
            int idEmpresa,
            int idSucursal,
            int? idIsla,
            int? idTrabajador,
            EDocumento? tipoDocumento,
            string? numSerie,
            string? numDocumento,
            ETipoMoneda? tipoMoneda,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT TOP (@PageSize)
                    d.IdDepositoBoveda,
                    d.IdEmpresa,
                    d.IdSucursal,
                    d.IdTrabajador,
                    d.IdIsla,
                    d.FechaEmision,
                    d.IdTipoDocumento  AS TipoDocumento,
                    d.NumSerie,
                    d.NumDocumento,
                    d.IdTipoMoneda     AS TipoMoneda,
                    d.Importe,
                    d.Glosa,
                    d.Estado
                FROM DepositoBoveda d
                WHERE d.IdEmpresa   = @IdEmpresa
                  AND d.IdSucursal  = @IdSucursal
                  AND (@IdIsla          IS NULL OR d.IdIsla          = @IdIsla)
                  AND (@IdTrabajador    IS NULL OR d.IdTrabajador    = @IdTrabajador)
                  AND (@TipoDocumento   IS NULL OR d.IdTipoDocumento = @TipoDocumento)
                  AND (@NumSerie        IS NULL OR d.NumSerie        = @NumSerie)
                  AND (@NumDocumento    IS NULL OR d.NumDocumento    = @NumDocumento)
                  AND (@TipoMoneda      IS NULL OR d.IdTipoMoneda    = @TipoMoneda)
                  AND (@FechaDesde      IS NULL OR d.FechaEmision   >= @FechaDesde)
                  AND (@FechaHasta      IS NULL OR d.FechaEmision   <= @FechaHasta)
                ORDER BY d.FechaEmision DESC, d.IdDepositoBoveda DESC
                """;

            var result = await connection.QueryAsync<VaultDepositSearchResult>(sql, new
            {
                PageSize = pageSize,
                IdEmpresa = idEmpresa,
                IdSucursal = idSucursal,
                IdIsla = idIsla,
                IdTrabajador = idTrabajador,
                TipoDocumento = tipoDocumento.HasValue ? (int?)tipoDocumento.Value : null,
                NumSerie = string.IsNullOrWhiteSpace(numSerie) ? null : numSerie.Trim(),
                NumDocumento = string.IsNullOrWhiteSpace(numDocumento) ? null : numDocumento.Trim(),
                TipoMoneda = tipoMoneda.HasValue ? (int?)tipoMoneda.Value : null,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta
            });

            return result.ToList();
        }

        public async Task<decimal> GetAvailableCashAsync(
            int idEmpresa,
            int idSucursal,
            int? idTrabajador,
            int? idIsla,
            ETipoMoneda tipoMoneda,
            DateTime fecha,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                DECLARE @Recaudado DECIMAL(18,2), @Depositado DECIMAL(18,2);

                SELECT @Recaudado = ISNULL(SUM(opd.Importe), 0)
                FROM OperacionPago op
                INNER JOIN OperacionPagoDetalle opd ON opd.IdOperacion = op.IdOperacion
                INNER JOIN Venta v ON v.IdVenta = op.IdVenta
                WHERE v.IdEmpresa  = @IdEmpresa
                  AND v.IdSucursal = @IdSucursal
                  AND opd.IdTipoMoneda = @TipoMoneda
                  AND CAST(v.FechaEmision AS DATE) = CAST(@Fecha AS DATE)
                  AND v.Estado <> 0
                  AND (@IdTrabajador IS NULL OR v.IdTrabajador = @IdTrabajador)
                  AND (@IdIsla IS NULL OR v.IdEstacion = @IdIsla);

                SELECT @Depositado = ISNULL(SUM(d.Importe), 0)
                FROM DepositoBoveda d
                WHERE d.IdEmpresa  = @IdEmpresa
                  AND d.IdSucursal = @IdSucursal
                  AND d.IdTipoMoneda = @TipoMoneda
                  AND d.Estado = 1
                  AND CAST(d.FechaEmision AS DATE) = CAST(@Fecha AS DATE)
                  AND (@IdTrabajador IS NULL OR d.IdTrabajador = @IdTrabajador)
                  AND (@IdIsla IS NULL OR d.IdIsla = @IdIsla);

                SELECT ISNULL(@Recaudado - @Depositado, 0);
                """;

            var result = await connection.ExecuteScalarAsync<decimal>(sql, new
            {
                IdEmpresa = idEmpresa,
                IdSucursal = idSucursal,
                IdTrabajador = idTrabajador,
                IdIsla = idIsla,
                TipoMoneda = (int)tipoMoneda,
                Fecha = fecha.Date
            });

            return result;
        }

        public void Update(VaultDeposit deposit)
        {
            DbContext.Update(deposit);
        }
    }
}
