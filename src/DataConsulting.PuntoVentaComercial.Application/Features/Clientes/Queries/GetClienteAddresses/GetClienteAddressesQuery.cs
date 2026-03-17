using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.GetClienteById;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.GetClienteAddresses;

public sealed record GetClienteAddressesQuery(int IdCliente)
    : IQuery<IList<ClienteLocalResponse>>;
