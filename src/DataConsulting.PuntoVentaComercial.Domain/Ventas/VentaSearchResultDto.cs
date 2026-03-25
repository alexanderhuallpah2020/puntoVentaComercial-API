namespace DataConsulting.PuntoVentaComercial.Domain.Ventas;

public sealed record VentaSearchResultDto(
    int IdVenta,
    short IdTipoDocumento,
    string? NumSerieA,
    string? NumeroDocumentoA,
    string ClienteNombre,
    short Vendedor,
    DateTime FechaEmision,
    string Estado,
    decimal ImporteTotal);
