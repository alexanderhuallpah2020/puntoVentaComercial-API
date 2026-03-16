namespace DataConsulting.PuntoVentaComercial.Application.Features.Auth.Commands.Login
{
    public sealed record LoginResponse(
        string Token,
        DateTimeOffset ExpiresAt,
        int IdUsuario,
        string Username,
        int IdTrabajador,
        string NombreTrabajador,
        int IdEmpresa,
        int IdSucursal,
        string NombreSucursal,
        int IdEstacion,
        string CodigoEstacion,
        List<string> Policies);
}
