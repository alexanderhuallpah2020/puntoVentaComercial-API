using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.LookupRucSunat
{
    public sealed record LookupRucSunatQuery(string Ruc) : IQuery<LookupRucSunatResponse>;
}
