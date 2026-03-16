namespace DataConsulting.PuntoVentaComercial.API.Controllers.Auth
{
    public sealed record LoginRequest(
        string Username,
        string Password,
        int IdEmpresa,
        string CodigoEstacion);
}
