using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetNextCorrelative
{
    public sealed record GetNextCorrelativeQuery(
        int IdEmpresa,
        int IdSucursal,
        EDocumento TipoDocumento,
        string NumSerie
    ) : IQuery<GetNextCorrelativeResponse>;
}
