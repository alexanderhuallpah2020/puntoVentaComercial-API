using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Products;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class ProductRepository(IDbConnectionFactory connectionFactory) : IProductRepository
    {
        public async Task<IReadOnlyList<ProductPriceListItem>> GetPriceListAsync(
            int idSucursal,
            int idTipoCliente,
            string? codigo,
            string? descripcion,
            bool soloConStock,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT
                    a.IdArticulo,
                    a.Codigo,
                    a.CodBarra,
                    a.Descripcion,
                    a.SiglaUnidad,
                    a.IdUnidad,
                    a.FactorUnd,
                    a.FlagCompuesto,
                    ISNULL(lp.Precio, a.PrecioVenta) AS PrecioVenta,
                    ISNULL(lp.ValorVenta, a.ValorVenta) AS ValorVenta,
                    ISNULL(s.StockDisponible, 0) AS StockDisponible,
                    a.IdAfectacionIgv AS TipoAfectacionIgv,
                    a.TasaIsc,
                    a.IdTipoIsc AS TipoIsc,
                    a.FlagIcbper,
                    a.IdClaseProducto
                FROM Articulo a
                LEFT JOIN ListaPrecioArticulo lp
                    ON lp.IdArticulo = a.IdArticulo
                    AND lp.IdTipoCliente = @IdTipoCliente
                LEFT JOIN (
                    SELECT IdArticulo, SUM(CantidadDisponible) AS StockDisponible
                    FROM StockAlmacen sa
                    INNER JOIN Almacen al ON al.IdAlmacen = sa.IdAlmacen AND al.IdSucursal = @IdSucursal
                    GROUP BY IdArticulo
                ) s ON s.IdArticulo = a.IdArticulo
                WHERE a.Estado = 1
                    AND (@Codigo IS NULL OR a.Codigo LIKE '%' + @Codigo + '%' OR a.CodBarra = @Codigo)
                    AND (@Descripcion IS NULL OR a.Descripcion LIKE '%' + @Descripcion + '%')
                    AND (@SoloConStock = 0 OR ISNULL(s.StockDisponible, 0) > 0)
                ORDER BY a.Descripcion
                """;

            var result = await connection.QueryAsync<ProductPriceListItem>(sql, new
            {
                IdSucursal = idSucursal,
                IdTipoCliente = idTipoCliente,
                Codigo = string.IsNullOrWhiteSpace(codigo) ? null : codigo.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim(),
                SoloConStock = soloConStock ? 1 : 0
            });

            return result.ToList();
        }

        public async Task<IReadOnlyList<ProductTopSellerItem>> GetTopSellersAsync(
            int idSucursal,
            int top,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT TOP (@Top)
                    a.IdArticulo,
                    a.Codigo,
                    a.Descripcion,
                    a.SiglaUnidad,
                    a.PrecioVenta,
                    ISNULL(s.StockDisponible, 0) AS StockDisponible,
                    a.IdAfectacionIgv AS TipoAfectacionIgv,
                    a.FlagIcbper,
                    SUM(vd.Cantidad) AS CantidadVendida
                FROM VentaDetalle vd
                INNER JOIN Articulo a ON a.IdArticulo = vd.IdArticulo
                INNER JOIN Venta v ON v.IdVenta = vd.IdVenta
                    AND v.IdSucursal = @IdSucursal
                    AND v.Estado = 1
                    AND v.FechaEmision >= DATEADD(DAY, -90, GETDATE())
                LEFT JOIN (
                    SELECT IdArticulo, SUM(CantidadDisponible) AS StockDisponible
                    FROM StockAlmacen sa
                    INNER JOIN Almacen al ON al.IdAlmacen = sa.IdAlmacen AND al.IdSucursal = @IdSucursal
                    GROUP BY IdArticulo
                ) s ON s.IdArticulo = a.IdArticulo
                WHERE a.Estado = 1
                GROUP BY a.IdArticulo, a.Codigo, a.Descripcion, a.SiglaUnidad, a.PrecioVenta,
                         s.StockDisponible, a.IdAfectacionIgv, a.FlagIcbper
                ORDER BY CantidadVendida DESC
                """;

            var result = await connection.QueryAsync<ProductTopSellerItem>(sql, new
            {
                IdSucursal = idSucursal,
                Top = top
            });

            return result.ToList();
        }

        public async Task<Product?> GetByIdWithComposicionAsync(
            int idArticulo,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sqlProduct = """
                SELECT
                    a.IdArticulo AS Id,
                    a.Codigo,
                    a.CodBarra,
                    a.Descripcion,
                    a.SiglaUnidad,
                    a.IdUnidad,
                    a.FactorUnd,
                    a.FlagCompuesto,
                    a.PrecioVenta,
                    a.ValorVenta,
                    0 AS StockDisponible,
                    a.IdAfectacionIgv AS TipoAfectacionIgv,
                    a.TasaIsc,
                    a.IdTipoIsc AS TipoIsc,
                    a.FlagIcbper,
                    a.IdClaseProducto
                FROM Articulo a
                WHERE a.IdArticulo = @IdArticulo AND a.Estado = 1
                """;

            var sqlComposicion = """
                SELECT
                    ca.IdArticuloComponente,
                    a.Codigo,
                    a.Descripcion,
                    a.SiglaUnidad,
                    ca.Cantidad,
                    a.IdAfectacionIgv AS TipoAfectacionIgv,
                    a.TasaIsc,
                    a.IdTipoIsc AS TipoIsc,
                    a.FlagIcbper
                FROM ComposicionArticulo ca
                INNER JOIN Articulo a ON a.IdArticulo = ca.IdArticuloComponente
                WHERE ca.IdArticulo = @IdArticulo
                """;

            // Ejecutar en paralelo
            var productTask = connection.QuerySingleOrDefaultAsync<dynamic>(sqlProduct, new { IdArticulo = idArticulo });
            var composicionTask = connection.QueryAsync<ProductComponent>(sqlComposicion, new { IdArticulo = idArticulo });

            await Task.WhenAll(productTask, composicionTask);

            var row = await productTask;
            if (row is null) return null;

            var composicion = (await composicionTask).ToList();

            return Product.Create(
                (int)row.Id,
                (string)row.Codigo,
                (string?)row.CodBarra,
                (string)row.Descripcion,
                (string)row.SiglaUnidad,
                (int)row.IdUnidad,
                (decimal)row.FactorUnd,
                (int)row.FlagCompuesto,
                (decimal)row.PrecioVenta,
                (decimal)row.ValorVenta,
                (decimal)row.StockDisponible,
                (int)row.TipoAfectacionIgv,
                (decimal)row.TasaIsc,
                (int)row.TipoIsc,
                (bool)row.FlagIcbper,
                (int)row.IdClaseProducto
            );
        }

        public async Task<decimal> GetStockAsync(
            int idArticulo,
            int idSucursal,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT ISNULL(SUM(sa.CantidadDisponible), 0)
                FROM StockAlmacen sa
                INNER JOIN Almacen al ON al.IdAlmacen = sa.IdAlmacen AND al.IdSucursal = @IdSucursal
                WHERE sa.IdArticulo = @IdArticulo
                """;

            return await connection.ExecuteScalarAsync<decimal>(sql, new
            {
                IdArticulo = idArticulo,
                IdSucursal = idSucursal
            });
        }
    }
}
