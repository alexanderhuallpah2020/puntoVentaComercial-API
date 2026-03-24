namespace DataConsulting.PuntoVentaComercial.API.Controllers.Ventas;

public sealed record CreateVentaRequest(
    short IdEmpresa,
    short IdSucursal,
    short IdEstacionTrabajo,
    short IdSubSede,
    short IdTipoDocumento,
    string NumSerieA,
    int IdCliente,
    short? IdTipoCliente,
    short IdVendedor,
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
    decimal RedondeoTotal,
    short IdFormaPago,
    short FlagDescPorcentaje,
    short? IdSubdiario,
    IList<CreateVentaDetalleItemRequest> Detalles,
    IList<CreateVentaPagoItemRequest> Pagos,
    IList<CreateVentaCuotaItemRequest> Cuotas,
    // VentaEmision
    string ClienteNombre,
    string ClienteDireccion,
    string Observacion,
    int PuntosBonus);

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
    decimal ValorICBPER,
    int IdLocacion);

public sealed record CreateVentaPagoItemRequest(
    short IdFormaPago,
    short IdTipoMoneda,
    decimal Importe);

public sealed record CreateVentaCuotaItemRequest(
    DateTime? FechaCuota,
    decimal? Monto);
