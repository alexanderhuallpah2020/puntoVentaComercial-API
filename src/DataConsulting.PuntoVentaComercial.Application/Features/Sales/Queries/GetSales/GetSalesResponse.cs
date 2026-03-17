namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetSales
{
    public sealed record GetSalesResponse(IReadOnlyList<SaleDto> Items);

    public sealed record SaleDto(
        int IdVenta,
        int IdEmpresa,
        int IdSucursal,
        DateTime FechaEmision,
        int TipoDocumento,
        string NumSerie,
        long Correlativo,
        string NumeroFormateado,
        int IdCliente,
        string NombreCliente,
        string NumDocumentoCliente,
        decimal ImporteTotal,
        int Estado);
}
