namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.SearchClients
{
    public sealed record SearchClientsResponse(IReadOnlyList<ClientSearchDto> Items);

    public sealed record ClientSearchDto(
        int IdCliente,
        string Nombre,
        string NombreComercial,
        int IdDocumentoIdentidad,
        string NumDocumento,
        string EstadoCliente,
        string DireccionLocal,
        string Telefono1);
}
