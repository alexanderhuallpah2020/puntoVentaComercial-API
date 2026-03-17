using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.GetClientAddresses
{
    internal sealed class GetClientAddressesQueryHandler(IClientRepository clientRepository)
        : IQueryHandler<GetClientAddressesQuery, GetClientAddressesResponse>
    {
        public async Task<Result<GetClientAddressesResponse>> Handle(
            GetClientAddressesQuery query,
            CancellationToken cancellationToken)
        {
            var locals = await clientRepository.GetLocalsAsync(query.IdCliente, cancellationToken);

            var dtos = locals.Select(l => new ClientAddressDto(
                l.IdLocal,
                l.IdCliente,
                l.IdSucursal,
                l.DireccionLocal,
                l.Telefono1,
                l.Estado)).ToList();

            return Result.Success(new GetClientAddressesResponse(dtos));
        }
    }
}
