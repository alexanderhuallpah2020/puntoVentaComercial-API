using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetShifts
{
    public sealed record GetShiftsQuery(
        int IdEmpresa,
        TimeOnly? HoraActual
    ) : IQuery<GetShiftsResponse>;
}
