using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Sales;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetSaleById
{
    internal sealed class GetSaleByIdQueryHandler(ISaleRepository saleRepository)
        : IQueryHandler<GetSaleByIdQuery, GetSaleByIdResponse>
    {
        public async Task<Result<GetSaleByIdResponse>> Handle(
            GetSaleByIdQuery query,
            CancellationToken cancellationToken)
        {
            var sale = await saleRepository.GetByIdAsync(query.IdVenta, cancellationToken);
            if (sale is null)
                return Result.Failure<GetSaleByIdResponse>(SaleErrors.NotFound(query.IdVenta));

            var items = sale.Items.Select(i => new SaleItemDto(
                i.IdDetalle,
                i.IdArticulo,
                i.Codigo,
                i.Descripcion,
                i.SiglaUnidad,
                i.Cantidad,
                i.PrecioUnitario,
                (int)i.TipoAfectacionIgv,
                i.ValorVenta,
                i.Descuento,
                i.Isc,
                i.Igv,
                i.Icbper,
                i.Subtotal)).ToList();

            return Result.Success(new GetSaleByIdResponse(
                sale.IdVenta,
                sale.IdEmpresa,
                sale.IdSucursal,
                sale.FechaEmision,
                sale.TipoDocumento,
                sale.NumSerie,
                sale.Correlativo,
                $"{sale.NumSerie}-{sale.Correlativo:D8}",
                sale.IdCliente,
                sale.NombreCliente,
                sale.IdDocumentoIdentidad,
                sale.NumDocumentoCliente,
                sale.DireccionCliente,
                sale.FlagIGV,
                sale.FormaPago,
                sale.TipoMoneda,
                sale.TipoCambio,
                sale.DescuentoGlobal,
                sale.ValorAfecto,
                sale.ValorInafecto,
                sale.ValorExonerado,
                sale.TotalIsc,
                sale.Igv,
                sale.TotalIcbper,
                sale.ImporteTotal,
                sale.Estado,
                items));
        }
    }
}
