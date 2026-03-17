using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.SearchClientes;

internal sealed class SearchClientesQueryHandler(IClienteRepository repository)
    : IQueryHandler<SearchClientesQuery, SearchClientesResponse>
{
    public async Task<Result<SearchClientesResponse>> Handle(
        SearchClientesQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repository.SearchAsync(
            request.Nombre, request.NumDocumento,
            request.IdPais, request.IdDocIdentidad,
            request.Page, request.PageSize, cancellationToken);

        var response = new SearchClientesResponse(
            items.Select(MapToResumen).ToList(),
            total, request.Page, request.PageSize);

        return Result.Success(response);
    }

    private static ClienteResumenResponse MapToResumen(Cliente c)
    {
        var local = c.ClienteLocales.FirstOrDefault();
        return new(c.Id, c.Nombre, c.NumDocumento, null,
            local?.DireccionLocal, local?.Telefono1, c.EstadoCliente);
    }
}
