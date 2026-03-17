namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.LookupRucSunat
{
    public sealed record LookupRucSunatResponse(
        string Ruc,
        string RazonSocial,
        string Direccion,
        string Ubigeo,
        string Departamento,
        string Provincia,
        string Distrito,
        bool Activo);
}
