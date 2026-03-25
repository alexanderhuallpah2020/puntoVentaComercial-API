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
            request.NombreCliente,
            request.NumSerieA,
            request.NumDocumento,
            request.IdTipoDocumento,
            request.Estado,
            request.Page,
            request.PageSize,
            cancellationToken);

        var response = new SearchVentasResponse(
            items.Select(v => new SearchVentasItemResponse(
                v.Id,
                v.IdTipoDocumento,
                v.NumSerieA,
                v.NumeroDocumentoA,
                v.Emision?.ClienteNombre ?? "",
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
