using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Services;

// TODO: reemplazar con lectura de claims JWT cuando se implemente Auth
internal sealed class CurrentUserService : ICurrentUserService
{
    public string UserName => "admin";
}
