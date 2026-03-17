using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Queries.GetPendingSubmissions
{
    public sealed record GetPendingSubmissionsQuery(
        int IdEmpresa,
        int IdSucursal,
        DateTime? FechaDesde,
        DateTime? FechaHasta,
        int PageSize = 100
    ) : IQuery<GetPendingSubmissionsResponse>;
}
