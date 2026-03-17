namespace DataConsulting.PuntoVentaComercial.API.Controllers.Clientes;

public sealed record UpdateClienteRequest(
    string Nombre,
    int? IdDocumentoIdentidad,
    string? NumDocumento,
    string? CodValidadorDoc,
    short IdPais,
    string DireccionLocal,
    string? Telefono1);
