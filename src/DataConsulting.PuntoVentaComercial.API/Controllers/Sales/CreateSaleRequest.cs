using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Sales
{
    public sealed record CreateSaleRequest(
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
        int? IdSubdiario,
        string? Observaciones,
        EFormaPago FormaPago,
        ETipoMoneda TipoMoneda,
        decimal TipoCambio,
        decimal DescuentoGlobal,
        List<CreateSaleItemRequest> Items,
        decimal ValorAfecto,
        decimal ValorInafecto,
        decimal ValorExonerado,
        decimal ValorGratuito,
        decimal TotalIsc,
        decimal Igv,
        decimal TotalIcbper,
        decimal ImporteTotal,
        int IdUsuarioCreador);

    public sealed record CreateSaleItemRequest(
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
