namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.LookupRucSunat;

public sealed record LookupRucSunatResponse(
    string Ruc,
    string RazonSocial,
    string Direccion,
    string Estado,
    string Condicion);
