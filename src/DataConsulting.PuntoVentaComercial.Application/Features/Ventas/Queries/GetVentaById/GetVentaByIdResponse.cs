namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Queries.GetVentaById;

public sealed record GetVentaByIdResponse(
    int IdVenta,
    short IdTipoDocumento,
    short? NumSerie,
    int? NumeroDocumento,
    int IdCliente,
    short Vendedor,
    DateTime FechaEmision,
    string Estado,
    short IdTipoMoneda,
    decimal ValorNeto,
    decimal ImporteDescuento,
    decimal ValorVenta,
    decimal Igv,
    decimal ValorExonerado,
    decimal Isc,
    decimal ValorICBPER,
    decimal ImporteTotal,
    decimal ImportePagado,
    decimal ImporteVuelto,
    IList<VentaDetalleResponse> Detalles,
    IList<VentaPagoResponse> Pagos);

public sealed record VentaDetalleResponse(
    short Correlativo,
    int IdArticulo,
    string? DescripcionArticulo,
    decimal Cantidad,
    decimal PrecioUnitario,
    decimal ImporteDescuento,
    decimal ValorVenta,
    bool FlagExonerado,
    int IdTipoAfectoIGV);

public sealed record VentaPagoResponse(
    short IdFormaPago,
    short IdTipoMoneda,
    decimal Importe);
