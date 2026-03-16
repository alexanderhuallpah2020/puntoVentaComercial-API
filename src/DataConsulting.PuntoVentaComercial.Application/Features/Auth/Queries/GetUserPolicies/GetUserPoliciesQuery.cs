using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Auth.Queries.GetUserPolicies
{
    public sealed record GetUserPoliciesQuery(int IdUsuario) : IQuery<GetUserPoliciesResponse>;
}
