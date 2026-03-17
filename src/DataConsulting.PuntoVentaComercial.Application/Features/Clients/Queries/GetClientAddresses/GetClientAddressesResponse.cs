namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.GetClientAddresses
{
    public sealed record GetClientAddressesResponse(IReadOnlyList<ClientAddressDto> Addresses);

    public sealed record ClientAddressDto(
        int IdLocal,
        int IdCliente,
        int IdSucursal,
        string DireccionLocal,
        string Telefono1,
        string Estado);
}
