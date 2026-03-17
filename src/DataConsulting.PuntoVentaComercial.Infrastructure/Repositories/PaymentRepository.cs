using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.Payments;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories
{
    internal sealed class PaymentRepository(
        ApplicationDbContext dbContext,
        IDbConnectionFactory connectionFactory)
        : Repository<Payment>(dbContext), IPaymentRepository
    {
        public async Task<Payment?> GetByIdVentaAsync(int idVenta, CancellationToken cancellationToken = default)
        {
            return await DbContext.Set<Payment>()
                .Include("_detalles")
                .FirstOrDefaultAsync(p => p.IdVenta == idVenta, cancellationToken);
        }

        public async Task<IReadOnlyList<PaymentMethodResult>> GetPaymentMethodsAsync(
            int idEmpresa,
            CancellationToken cancellationToken = default)
        {
            await using var connection = await connectionFactory.OpenConnectionAsync();

            var sql = """
                SELECT
                    IdFormaPago,
                    Descripcion,
                    CAST(Activo AS BIT) AS Activo
                FROM FormaPago
                WHERE IdEmpresa = @IdEmpresa AND Activo = 1
                ORDER BY Descripcion
                """;

            var result = await connection.QueryAsync<PaymentMethodResult>(sql, new
            {
                IdEmpresa = idEmpresa
            });

            return result.ToList();
        }
    }
}
