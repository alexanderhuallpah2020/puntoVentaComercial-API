using Dapper;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Sales;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetPrintableSale
{
    internal sealed class GetPrintableSaleQueryHandler(
        ISaleRepository saleRepository,
        IDbConnectionFactory connectionFactory)
        : IQueryHandler<GetPrintableSaleQuery, GetPrintableSaleResponse>
    {
        public async Task<Result<GetPrintableSaleResponse>> Handle(
            GetPrintableSaleQuery query,
            CancellationToken cancellationToken)
        {
            var sale = await saleRepository.GetByIdAsync(query.IdVenta, cancellationToken);

            if (sale is null)
                return Result.Failure<GetPrintableSaleResponse>(SaleErrors.NotFound(query.IdVenta));

            var (nombre, ruc, direccion) = await GetEmpresaInfoAsync(sale.IdEmpresa);

            var items = sale.Items.Select(i => new PrintItemDto(
                i.Codigo,
                i.Descripcion,
                i.SiglaUnidad,
                i.Cantidad,
                i.PrecioUnitario,
                i.Descuento,
                i.Subtotal)).ToList();

            var response = new GetPrintableSaleResponse(
                NombreEmpresa: nombre,
                RucEmpresa: ruc,
                DireccionEmpresa: direccion,
                IdVenta: sale.IdVenta,
                TipoDocumento: sale.TipoDocumento,
                NumeroFormateado: $"{sale.NumSerie}-{sale.Correlativo:D8}",
                FechaEmision: sale.FechaEmision,
                IdDocumentoIdentidad: sale.IdDocumentoIdentidad,
                NombreCliente: sale.NombreCliente,
                NumDocumentoCliente: sale.NumDocumentoCliente,
                DireccionCliente: sale.DireccionCliente,
                FlagIGV: sale.FlagIGV,
                TipoMoneda: sale.TipoMoneda,
                DescuentoGlobal: sale.DescuentoGlobal,
                ValorAfecto: sale.ValorAfecto,
                ValorInafecto: sale.ValorInafecto,
                ValorExonerado: sale.ValorExonerado,
                ValorGratuito: sale.ValorGratuito,
                TotalIsc: sale.TotalIsc,
                Igv: sale.Igv,
                TotalIcbper: sale.TotalIcbper,
                ImporteTotal: sale.ImporteTotal,
                Observaciones: sale.Observaciones,
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
