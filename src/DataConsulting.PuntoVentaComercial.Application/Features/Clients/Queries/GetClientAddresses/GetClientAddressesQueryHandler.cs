using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.GetClientAddresses
{
    internal sealed class GetClientAddressesQueryHandler(
        IClientRepository clientRepository)
        : IQueryHandler<GetClientAddressesQuery, List<ClientAddressResponse>>
    {
        public async Task<Result<List<ClientAddressResponse>>> Handle(
            GetClientAddressesQuery request,
            CancellationToken cancellationToken)
        {
            List<ClientLocal> addresses = await clientRepository.GetAddressesAsync(
                request.IdCliente,
                cancellationToken);

            List<ClientAddressResponse> response = addresses.Select(a => new ClientAddressResponse(
                a.IdLocal,
                a.IdCliente,
                a.DireccionLocal,
                a.Telefono1,
                a.IdTipoCliente,
                a.IdSucursal,
                a.IdRuta,
                a.Estado)).ToList();

            return Result.Success(response);
        }
    }
}
