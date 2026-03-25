namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Queries.SearchVentas;

public sealed record SearchVentasResponse(
    IList<SearchVentasItemResponse> Items,
    int Total,
    int Page,
    int PageSize);

public sealed record SearchVentasItemResponse(
    int IdVenta,
    short IdTipoDocumento,
    string? NumSerieA,
    string? NumeroDocumentoA,
    string ClienteNombre,
    short Vendedor,
    DateTime FechaEmision,
    string Estado,
    decimal ImporteTotal);
