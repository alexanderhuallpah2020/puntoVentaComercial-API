using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Queries.GetAvailableCash
{
    public sealed record GetAvailableCashQuery(
        int IdEmpresa,
        int IdSucursal,
        int? IdTrabajador,
        int? IdIsla,
        ETipoMoneda TipoMoneda,
        DateTime Fecha
    ) : IQuery<GetAvailableCashResponse>;
}
