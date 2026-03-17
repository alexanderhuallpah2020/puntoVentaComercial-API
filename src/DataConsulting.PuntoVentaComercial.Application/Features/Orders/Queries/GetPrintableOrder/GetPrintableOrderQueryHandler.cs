using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetPrintableSale;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Orders;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetPrintableOrder
{
    internal sealed class GetPrintableOrderQueryHandler(
        IOrderRepository orderRepository,
        IDbConnectionFactory connectionFactory)
        : IQueryHandler<GetPrintableOrderQuery, GetPrintableOrderResponse>
    {
        public async Task<Result<GetPrintableOrderResponse>> Handle(
            GetPrintableOrderQuery query,
            CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(query.IdPedido, cancellationToken);

            if (order is null)
                return Result.Failure<GetPrintableOrderResponse>(OrderErrors.NotFound(query.IdPedido));

            var (nombre, ruc, direccion) = await GetEmpresaInfoAsync(order.IdEmpresa);

            var items = order.Items.Select(i => new PrintItemDto(
                i.Codigo,
                i.Descripcion,
                i.SiglaUnidad,
                i.Cantidad,
                i.PrecioUnitario,
                i.Descuento,
                i.Subtotal)).ToList();

            var response = new GetPrintableOrderResponse(
                NombreEmpresa: nombre,
                RucEmpresa: ruc,
                DireccionEmpresa: direccion,
                IdPedido: order.IdPedido,
                TipoDocumento: order.TipoDocumento,
                NumeroFormateado: $"{order.NumSerie}-{order.Correlativo:D8}",
                FechaEmision: order.FechaEmision,
                IdDocumentoIdentidad: order.IdDocumentoIdentidad,
                NombreCliente: order.NombreCliente,
                NumDocumentoCliente: order.NumDocumentoCliente,
                DireccionCliente: order.DireccionCliente,
                FlagIGV: order.FlagIGV,
                TipoMoneda: order.TipoMoneda,
                DescuentoGlobal: order.DescuentoGlobal,
                ValorAfecto: order.ValorAfecto,
                ValorInafecto: order.ValorInafecto,
                ValorExonerado: order.ValorExonerado,
                ValorGratuito: order.ValorGratuito,
                TotalIsc: order.TotalIsc,
                Igv: order.Igv,
                TotalIcbper: order.TotalIcbper,
                ImporteTotal: order.ImporteTotal,
                Observaciones: order.Observaciones,
                Items: items);

            return Result.Success(response);
        }

        private async Task<(string Nombre, string Ruc, string Direccion)> GetEmpresaInfoAsync(int idEmpresa)
        {
            try
            {
                await using var connection = await connectionFactory.OpenConnectionAsync();
                var info = await connection.QueryFirstOrDefaultAsync<EmpresaRow>(
                    "SELECT RazonSocial, NumRUC, Direccion FROM Empresa WHERE IdEmpresa = @IdEmpresa",
                    new { IdEmpresa = idEmpresa });

                return info is null
                    ? (string.Empty, string.Empty, string.Empty)
                    : (info.RazonSocial ?? string.Empty, info.NumRUC ?? string.Empty, info.Direccion ?? string.Empty);
            }
            catch
            {
                return (string.Empty, string.Empty, string.Empty);
            }
        }

        private sealed record EmpresaRow(string? RazonSocial, string? NumRUC, string? Direccion);
    }
}
