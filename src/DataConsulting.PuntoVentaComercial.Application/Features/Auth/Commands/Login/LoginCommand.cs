using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Auth.Commands.Login
{
    public sealed record LoginCommand(
        string Username,
        string Password,
        int IdEmpresa,
        string CodigoEstacion) : ICommand<LoginResponse>;
}
