using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.SearchClients
{
    public sealed record ClientSummaryResponse(
        int IdCliente,
        string Nombre,
        string? NombreComercial,
        EDocumentoIdentidad TipoDocumento,
        string NumDocumento,
        string Direccion,
        EEstado Estado);
}
