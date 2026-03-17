using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.GetClienteById;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.GetClienteAddresses;

internal sealed class GetClienteAddressesQueryHandler(IClienteRepository repository)
    : IQueryHandler<GetClienteAddressesQuery, IList<ClienteLocalResponse>>
{
    public async Task<Result<IList<ClienteLocalResponse>>> Handle(
        GetClienteAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await repository.GetAddressesByClienteIdAsync(request.IdCliente, cancellationToken);

        var response = addresses.Select(l => new ClienteLocalResponse(
            l.Id, l.IdSucursal, l.DireccionLocal, l.Telefono1, l.Estado)).ToList();

        return Result.Success<IList<ClienteLocalResponse>>(response);
    }
}
