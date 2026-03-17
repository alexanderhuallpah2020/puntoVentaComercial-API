using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Orders;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class OrderRepository(
        ApplicationDbContext dbContext,
        IDbConnectionFactory connectionFactory)
        : Repository<Order>(dbContext), IOrderRepository
    {
        public new async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<Order>()
                .Include("_items")
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<OrderSearchResult>> SearchAsync(
            int idEmpresa,
            int idSucursal,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int? idCliente,
            int? idTrabajador,
            int? estado,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT TOP (@PageSize)
                    p.IdPedido,
                    p.IdEmpresa,
                    p.IdSucursal,
                    p.FechaEmision,
                    p.IdTipoDocumento   AS TipoDocumento,
                    p.NumSerie,
                    p.Correlativo,
                    p.IdCliente,
                    p.NombreCliente,
                    p.NumDocumentoCliente,
                    p.ImporteTotal,
                    p.Estado
                FROM Pedido p
                WHERE p.IdEmpresa  = @IdEmpresa
                  AND p.IdSucursal = @IdSucursal
                  AND (@FechaDesde   IS NULL OR p.FechaEmision  >= @FechaDesde)
                  AND (@FechaHasta   IS NULL OR p.FechaEmision  <= @FechaHasta)
                  AND (@IdCliente    IS NULL OR p.IdCliente      = @IdCliente)
                  AND (@IdTrabajador IS NULL OR p.IdTrabajador   = @IdTrabajador)
                  AND (@Estado       IS NULL OR p.Estado         = @Estado)
                ORDER BY p.FechaEmision DESC, p.IdPedido DESC
                """;

            var result = await connection.QueryAsync<OrderSearchResult>(sql, new
            {
                PageSize = pageSize,
                IdEmpresa = idEmpresa,
                IdSucursal = idSucursal,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta,
                IdCliente = idCliente,
                IdTrabajador = idTrabajador,
                Estado = estado
            });

            return result.ToList();
        }

        public void Update(Order order)
        {
            DbContext.Update(order);
        }
    }
}
