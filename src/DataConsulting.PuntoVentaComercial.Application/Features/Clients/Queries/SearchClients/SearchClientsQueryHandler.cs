using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.SearchClients
{
    internal sealed class SearchClientsQueryHandler(
        IClientRepository clientRepository)
        : IQueryHandler<SearchClientsQuery, List<ClientSummaryResponse>>
    {
        public async Task<Result<List<ClientSummaryResponse>>> Handle(
            SearchClientsQuery request,
            CancellationToken cancellationToken)
        {
            List<Client> clients = await clientRepository.SearchAsync(
                request.Nombre,
                request.NumDocumento,
                request.TipoDocumento,
                request.IdEmpresa,
                cancellationToken);

            List<ClientSummaryResponse> response = clients.Select(c => new ClientSummaryResponse(
                c.IdCliente,
                c.Nombre,
                c.NombreComercial,
                c.IdDocumentoIdentidad,
                c.NumDocumento,
                c.Direccion,
                c.EstadoCliente)).ToList();

            return Result.Success(response);
        }
    }
}
