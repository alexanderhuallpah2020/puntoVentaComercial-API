namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.LookupRucSunat
{
    public sealed record RucSunatResponse(
        string Ruc,
        string RazonSocial,
        string Direccion,
        string Estado,
        string TipoContribuyente);
}
