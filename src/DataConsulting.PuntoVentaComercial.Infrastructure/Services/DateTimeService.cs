using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Services;

internal sealed class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
}
