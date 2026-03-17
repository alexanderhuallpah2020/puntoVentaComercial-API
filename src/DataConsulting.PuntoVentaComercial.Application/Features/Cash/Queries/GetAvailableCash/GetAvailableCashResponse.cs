using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Queries.GetAvailableCash
{
    public sealed record GetAvailableCashResponse(
        decimal Disponible,
        ETipoMoneda TipoMoneda,
        DateTime Fecha);
}
