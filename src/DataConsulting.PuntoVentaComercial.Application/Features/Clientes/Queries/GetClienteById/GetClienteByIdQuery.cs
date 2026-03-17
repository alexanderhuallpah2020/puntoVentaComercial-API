using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.GetClienteById;

public sealed record GetClienteByIdQuery(int IdCliente) : IQuery<GetClienteByIdResponse>;
