using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Auth.Queries.GetSessionEnvironment
{
    public sealed record GetSessionEnvironmentQuery(
        int IdEmpresa,
        int IdSucursal,
        int IdEstacion,
        int IdTrabajador) : IQuery<GetSessionEnvironmentResponse>;
}
