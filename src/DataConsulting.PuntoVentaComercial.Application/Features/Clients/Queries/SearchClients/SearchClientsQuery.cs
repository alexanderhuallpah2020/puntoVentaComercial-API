using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.SearchClients
{
    public sealed record SearchClientsQuery(
        string? Nombre,
        string? NumDocumento,
        EDocumentoIdentidad? TipoDocumento,
        int IdEmpresa
    ) : IQuery<List<ClientSummaryResponse>>;
}
