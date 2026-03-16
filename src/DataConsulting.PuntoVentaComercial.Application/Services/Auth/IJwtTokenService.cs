using DataConsulting.PuntoVentaComercial.Domain.Identity;

namespace DataConsulting.PuntoVentaComercial.Application.Services.Auth
{
    public interface IJwtTokenService
    {
        (string Token, DateTimeOffset ExpiresAt) GenerateToken(
            User user,
            Workstation workstation,
            List<string> policies);
    }
}
