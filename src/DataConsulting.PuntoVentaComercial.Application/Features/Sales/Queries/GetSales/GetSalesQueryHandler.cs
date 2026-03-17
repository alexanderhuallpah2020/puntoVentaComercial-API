using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Sales;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetSales
{
    internal sealed class GetSalesQueryHandler(ISaleRepository saleRepository)
        : IQueryHandler<GetSalesQuery, GetSalesResponse>
    {
        public async Task<Result<GetSalesResponse>> Handle(
            GetSalesQuery query,
            CancellationToken cancellationToken)
        {
            var results = await saleRepository.SearchAsync(
                query.IdEmpresa,
                query.IdSucursal,
                query.FechaDesde,
                query.FechaHasta,
                query.IdTipoDocumento,
                query.NumSerie,
                query.Correlativo,
                query.IdCliente,
                query.Estado,
                query.PageSize,
                cancellationToken);

            var dtos = results.Select(r => new SaleDto(
                r.IdVenta,
                r.IdEmpresa,
                r.IdSucursal,
                r.FechaEmision,
                r.TipoDocumento,
                r.NumSerie,
                r.Correlativo,
                $"{r.NumSerie}-{r.Correlativo:D8}",
                r.IdCliente,
                r.NombreCliente,
                r.NumDocumentoCliente,
                r.ImporteTotal,
                r.Estado)).ToList();

            return Result.Success(new GetSalesResponse(dtos));
        }
    }
}
