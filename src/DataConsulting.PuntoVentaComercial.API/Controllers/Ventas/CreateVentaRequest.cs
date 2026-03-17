namespace DataConsulting.PuntoVentaComercial.API.Controllers.Ventas;

public sealed record CreateVentaRequest(
    short IdEmpresa,
    short IdSucursal,
    short IdEstacionTrabajo,
    short IdSubSede,
    short IdTipoDocumento,
    short NumSerie,
    int IdCliente,
    short? IdTipoCliente,
    short IdVendedor,
    short? IdVendedor2,
    short IdTurnoAsistencia,
    short IdTipoMoneda,
    decimal TipoCambio,
    decimal ValorNeto,
    decimal ImporteDescuento,
    decimal ImporteDescuentoGlobal,
    decimal PorcentajeDescuentoGlobal,
    decimal ValorVenta,
    decimal Igv,
    decimal ValorExonerado,
    decimal Isc,
    decimal ValorICBPER,
    decimal ImporteTotal,
    decimal ImportePagado,
    decimal ImporteVuelto,
    decimal Redondeo,
    short IdFormaPago,
    short FlagDescPorcentaje,
    IList<CreateVentaDetalleItemRequest> Detalles,
    IList<CreateVentaPagoItemRequest> Pagos);

public sealed record CreateVentaDetalleItemRequest(
    int IdArticulo,
    short IdUnidad,
    string? DescripcionArticulo,
    decimal Cantidad,
    decimal PrecioUnitario,
    decimal ImporteDescuento,
    byte TipoDescuento,
    bool FlagExonerado,
    byte FlagRegalo,
    int IdTipoAfectoIGV,
    decimal Isc,
    decimal ValorICBPER);

public sealed record CreateVentaPagoItemRequest(
    short IdFormaPago,
    short IdTipoMoneda,
    decimal Importe);
