namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.GetClienteById;

public sealed record GetClienteByIdResponse(
    int IdCliente,
    string Nombre,
    string? NombreComercial,
    int? IdDocumentoIdentidad,
    string? NumDocumento,
    string? CodValidadorDoc,
    short IdPais,
    string EstadoCliente,
    bool EsEditableDesdePos,
    IList<ClienteLocalResponse> ClienteLocales);

public sealed record ClienteLocalResponse(
    int IdClienteLocal,
    short IdSucursal,
    string DireccionLocal,
    string? Telefono1,
    string Estado);
