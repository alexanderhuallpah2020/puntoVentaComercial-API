using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Queries.GetSubmissionStatus
{
    public sealed record GetSubmissionStatusQuery(int IdVenta) : IQuery<GetSubmissionStatusResponse>;
}
