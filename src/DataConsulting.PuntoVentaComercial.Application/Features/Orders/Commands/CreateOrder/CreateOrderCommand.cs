using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Commands.CreateOrder
{
    public sealed record CreateOrderCommand(
        EDocumento TipoDocumento,
        string NumSerie,
        int IdCliente,
        string NombreCliente,
        int IdDocumentoIdentidad,
        string NumDocumentoCliente,
        string DireccionCliente,
        bool FlagIGV,
        int IdEmpresa,
        int IdSucursal,
        int IdEstacion,
        int IdTurnoAsistencia,
        int IdTrabajador,
        int? IdTrabajador2,
        string? Observaciones,
        ETipoMoneda TipoMoneda,
        decimal TipoCambio,
        decimal DescuentoGlobal,
        List<CreateOrderItemCommand> Items,
        decimal ValorAfecto,
        decimal ValorInafecto,
        decimal ValorExonerado,
        decimal ValorGratuito,
        decimal TotalIsc,
        decimal Igv,
        decimal TotalIcbper,
        decimal ImporteTotal,
        int IdUsuarioCreador
    ) : ICommand<CreateOrderResponse>;

    public sealed record CreateOrderItemCommand(
        int IdArticulo,
        string Codigo,
        string Descripcion,
        string SiglaUnidad,
        int IdUnidad,
        decimal Cantidad,
        decimal PrecioUnitario,
        ETipoAfectacionIgv TipoAfectacionIgv,
        decimal ValorVenta,
        decimal Descuento,
        decimal Isc,
        decimal Igv,
        decimal Icbper,
        decimal Subtotal,
        int IdClaseProducto,
        int IdTipoCliente);
}
