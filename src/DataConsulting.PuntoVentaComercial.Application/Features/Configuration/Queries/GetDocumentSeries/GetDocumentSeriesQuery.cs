using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetDocumentSeries
{
    public sealed record GetDocumentSeriesQuery(
        int IdEmpresa,
        int IdSucursal,
        int IdEstacion
    ) : IQuery<GetDocumentSeriesResponse>;
}
