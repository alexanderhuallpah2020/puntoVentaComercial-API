using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetConstants
{
    public sealed record GetConstantsQuery(
        int IdEmpresa,
        int IdSucursal
    ) : IQuery<GetConstantsResponse>;
}
