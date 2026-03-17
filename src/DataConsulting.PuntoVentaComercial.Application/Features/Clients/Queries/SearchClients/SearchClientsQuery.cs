using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.SearchClients
{
    public sealed record SearchClientsQuery(
        string? Nombre,
        string? NumDocumento,
        int? IdDocumentoIdentidad,
        int PageSize = 50
    ) : IQuery<SearchClientsResponse>;
}
