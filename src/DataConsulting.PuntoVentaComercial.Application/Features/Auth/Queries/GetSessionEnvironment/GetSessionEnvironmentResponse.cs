namespace DataConsulting.PuntoVentaComercial.Application.Features.Auth.Queries.GetSessionEnvironment
{
    public sealed record GetSessionEnvironmentResponse(
        int IdEmpresa,
        string NombreEmpresa,
        string RucEmpresa,
        int IdSucursal,
        string NombreSucursal,
        int IdEstacion,
        string CodigoEstacion,
        int IdTrabajador,
        string NombreTrabajador,
        decimal MontoMaximoBoleta,
        bool UsaIgv,
        bool UsaIsc);
}
