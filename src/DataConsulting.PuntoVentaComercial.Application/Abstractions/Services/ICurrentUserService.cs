namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

public interface ICurrentUserService
{
    // Nombre de usuario en sesión — "admin" hasta implementar JWT
    string UserName { get; }
}
