using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.GetClientAddresses
{
    public sealed record ClientAddressResponse(
        int IdLocal,
        int IdCliente,
        string DireccionLocal,
        string Telefono1,
        int IdTipoCliente,
        int IdSucursal,
        int? IdRuta,
        EEstado Estado);
}
