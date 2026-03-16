using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.GetClientById
{
    public sealed record GetClientByIdQuery(int IdCliente) : IQuery<ClientDetailResponse>;
}
