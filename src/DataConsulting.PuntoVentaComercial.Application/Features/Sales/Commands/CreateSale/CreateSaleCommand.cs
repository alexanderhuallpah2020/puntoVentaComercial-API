using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CreateSale
{
    public sealed record CreateSaleCommand(
        // Documento
        EDocumento TipoDocumento,
        string NumSerie,
        // Cliente
        int IdCliente,
        string NombreCliente,
        int IdDocumentoIdentidad,
        string NumDocumentoCliente,
        string DireccionCliente,
        bool FlagIGV,
        // Sesión / contexto
        int IdEmpresa,
        int IdSucursal,
        int IdEstacion,
        int IdTurnoAsistencia,
        int IdTrabajador,
        int? IdTrabajador2,
        int? IdSubdiario,
        string? Observaciones,
        // Condiciones comerciales
        EFormaPago FormaPago,
        ETipoMoneda TipoMoneda,
        decimal TipoCambio,
        decimal DescuentoGlobal,
        // Ítems (con montos ya calculados por CalculateSale)
        List<CreateSaleItemCommand> Items,
        // Totales pre-calculados
        decimal ValorAfecto,
        decimal ValorInafecto,
        decimal ValorExonerado,
        decimal ValorGratuito,
        decimal TotalIsc,
        decimal Igv,
        decimal TotalIcbper,
        decimal ImporteTotal,
        // Auditoría
        int IdUsuarioCreador
    ) : ICommand<CreateSaleResponse>;

    public sealed record CreateSaleItemCommand(
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
