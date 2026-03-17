using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.SearchClients
{
    internal sealed class SearchClientsQueryHandler(IClientRepository clientRepository)
        : IQueryHandler<SearchClientsQuery, SearchClientsResponse>
    {
        public async Task<Result<SearchClientsResponse>> Handle(
            SearchClientsQuery query,
            CancellationToken cancellationToken)
        {
            var results = await clientRepository.SearchAsync(
                query.Nombre,
                query.NumDocumento,
                query.IdDocumentoIdentidad,
                query.PageSize,
                cancellationToken);

            var dtos = results.Select(r => new ClientSearchDto(
                r.IdCliente,
                r.Nombre,
                r.NombreComercial,
                r.IdDocumentoIdentidad,
                r.NumDocumento,
                r.EstadoCliente,
                r.DireccionLocal,
                r.Telefono1)).ToList();

            return Result.Success(new SearchClientsResponse(dtos));
        }
    }
}
