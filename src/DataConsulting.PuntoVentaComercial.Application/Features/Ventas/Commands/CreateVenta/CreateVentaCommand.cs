using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.CreateVenta;

public sealed record CreateVentaCommand(
    short IdEmpresa,
    short IdSucursal,
    short IdEstacionTrabajo,
    short IdSubSede,
    short IdTipoDocumento,
    short? NumSerie,
    string NumSerieA,
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
    decimal RedondeoTotal,
    short IdFormaPago,
    short FlagDescPorcentaje,
    short? IdSubdiario,
    IList<CreateVentaDetalleDto> Detalles,
    IList<CreateVentaPagoDto> Pagos) : ICommand<int>;

public sealed record CreateVentaDetalleDto(
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

public sealed record CreateVentaPagoDto(
    short IdFormaPago,
    short IdTipoMoneda,
    decimal Importe);
