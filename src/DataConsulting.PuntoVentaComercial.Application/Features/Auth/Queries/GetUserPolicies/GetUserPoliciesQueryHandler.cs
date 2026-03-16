using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Identity;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Auth.Queries.GetUserPolicies
{
    internal sealed class GetUserPoliciesQueryHandler(IUserRepository userRepository)
        : IQueryHandler<GetUserPoliciesQuery, GetUserPoliciesResponse>
    {
        public async Task<Result<GetUserPoliciesResponse>> Handle(
            GetUserPoliciesQuery query,
            CancellationToken cancellationToken)
        {
            var policies = await userRepository.GetPoliciesAsync(query.IdUsuario, cancellationToken);

            return Result.Success(new GetUserPoliciesResponse(policies));
        }
    }
}
