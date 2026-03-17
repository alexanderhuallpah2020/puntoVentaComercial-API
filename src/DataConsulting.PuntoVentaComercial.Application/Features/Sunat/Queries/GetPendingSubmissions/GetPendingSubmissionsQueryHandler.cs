using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Sunat;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Queries.GetPendingSubmissions
{
    internal sealed class GetPendingSubmissionsQueryHandler(ISunatSubmissionRepository repository)
        : IQueryHandler<GetPendingSubmissionsQuery, GetPendingSubmissionsResponse>
    {
        public async Task<Result<GetPendingSubmissionsResponse>> Handle(
            GetPendingSubmissionsQuery query,
            CancellationToken cancellationToken)
        {
            var items = await repository.GetPendingAsync(
                query.IdEmpresa,
                query.IdSucursal,
                query.FechaDesde,
                query.FechaHasta,
                query.PageSize,
                cancellationToken);

            return Result.Success(new GetPendingSubmissionsResponse(items));
        }
    }
}
