using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Ventas;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Queries.SearchVentas;

internal sealed class SearchVentasQueryHandler(IVentaRepository repository)
    : IQueryHandler<SearchVentasQuery, SearchVentasResponse>
{
    public async Task<Result<SearchVentasResponse>> Handle(
        SearchVentasQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repository.SearchAsync(
            request.FechaDesde,
            request.FechaHasta,
            request.IdCliente,
            request.NumSerie,
            request.IdTipoDocumento,
            request.Estado,
            request.IdSucursal,
            request.Page,
            request.PageSize,
            cancellationToken);

        var response = new SearchVentasResponse(
            items.Select(v => new SearchVentasItemResponse(
                v.Id,
                v.IdTipoDocumento,
                v.NumSerie,
                v.NumeroDocumento,
                v.IdCliente,
                v.Vendedor,
                v.FechaEmision,
                v.Estado,
                v.ImporteTotal)).ToList(),
            total,
            request.Page,
            request.PageSize);

        return Result.Success(response);
    }
}
