namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Queries.SearchVentas;

public sealed record SearchVentasResponse(
    IList<SearchVentasItemResponse> Items,
    int Total,
    int Page,
    int PageSize);

public sealed record SearchVentasItemResponse(
    int IdVenta,
    short IdTipoDocumento,
    short? NumSerie,
    int? NumeroDocumento,
    int IdCliente,
    short Vendedor,
    DateTime FechaEmision,
    string Estado,
    decimal ImporteTotal);
