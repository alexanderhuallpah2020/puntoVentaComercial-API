using DataConsulting.PuntoVentaComercial.Domain.Sunat;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Queries.GetPendingSubmissions
{
    public sealed record GetPendingSubmissionsResponse(IReadOnlyList<PendingSubmissionResult> Items);
}
