namespace DataConsulting.PuntoVentaComercial.API.Controllers.Clientes;

public sealed record CreateClienteRequest(
    string Nombre,
    int? IdDocumentoIdentidad,
    string? NumDocumento,
    string? CodValidadorDoc,
    short IdPais,
    string DireccionLocal,
    string? Telefono1,
    short IdSucursal,
    string? NombreComercial = null);
