namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetOrders
{
    public sealed record GetOrdersResponse(IReadOnlyList<OrderDto> Items);

    public sealed record OrderDto(
        int IdPedido,
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
