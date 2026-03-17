using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Sales;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class SaleRepository(
        ApplicationDbContext dbContext,
        IDbConnectionFactory connectionFactory)
        : Repository<Sale>(dbContext), ISaleRepository
    {
        public new async Task<Sale?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<Sale>()
                .Include("_items")
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<SaleSearchResult>> SearchAsync(
            int idEmpresa,
            int idSucursal,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int? idTipoDocumento,
            string? numSerie,
            long? correlativo,
            int? idCliente,
            int? estado,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT TOP (@PageSize)
                    v.IdVenta,
                    v.IdEmpresa,
                    v.IdSucursal,
                    v.FechaEmision,
                    v.IdTipoDocumento   AS TipoDocumento,
                    v.NumSerie,
                    v.Correlativo,
                    v.IdCliente,
                    v.NombreCliente,
                    v.NumDocumentoCliente,
                    v.ImporteTotal,
                    v.Estado
                FROM Venta v
                WHERE v.IdEmpresa  = @IdEmpresa
                  AND v.IdSucursal = @IdSucursal
                  AND (@FechaDesde       IS NULL OR v.FechaEmision    >= @FechaDesde)
                  AND (@FechaHasta       IS NULL OR v.FechaEmision    <= @FechaHasta)
                  AND (@IdTipoDocumento  IS NULL OR v.IdTipoDocumento  = @IdTipoDocumento)
                  AND (@NumSerie         IS NULL OR v.NumSerie         = @NumSerie)
                  AND (@Correlativo      IS NULL OR v.Correlativo      = @Correlativo)
                  AND (@IdCliente        IS NULL OR v.IdCliente        = @IdCliente)
                  AND (@Estado           IS NULL OR v.Estado           = @Estado)
                ORDER BY v.FechaEmision DESC, v.IdVenta DESC
                """;

            var result = await connection.QueryAsync<SaleSearchResult>(sql, new
            {
                PageSize = pageSize,
                IdEmpresa = idEmpresa,
                IdSucursal = idSucursal,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                IdTipoDocumento = idTipoDocumento,
                NumSerie = string.IsNullOrWhiteSpace(numSerie) ? null : numSerie.Trim(),
                Correlativo = correlativo,
                IdCliente = idCliente,
                Estado = estado
            });

            return result.ToList();
        }

        public void Update(Sale sale)
        {
            DbContext.Update(sale);
        }
    }
}
