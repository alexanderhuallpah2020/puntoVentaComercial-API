namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.SearchClientes;

public sealed record SearchClientesResponse(
    IList<ClienteResumenResponse> Items,
    int Total,
    int Page,
    int PageSize);

public sealed record ClienteResumenResponse(
    int IdCliente,
    string Nombre,
    string? NumDocumento,
    string? TipoDocumento,
    string? DireccionLocal,
    string? Telefono1,
    string EstadoCliente);
