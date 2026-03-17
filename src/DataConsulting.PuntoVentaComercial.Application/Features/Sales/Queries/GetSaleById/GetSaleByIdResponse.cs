using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetSaleById
{
    public sealed record GetSaleByIdResponse(
        int IdVenta,
        int IdEmpresa,
        int IdSucursal,
        DateTime FechaEmision,
        EDocumento TipoDocumento,
        string NumSerie,
        long Correlativo,
        string NumeroFormateado,
        int IdCliente,
        string NombreCliente,
        int IdDocumentoIdentidad,
        string NumDocumentoCliente,
        string DireccionCliente,
        bool FlagIGV,
        EFormaPago FormaPago,
        ETipoMoneda TipoMoneda,
        decimal TipoCambio,
        decimal DescuentoGlobal,
        decimal ValorAfecto,
        decimal ValorInafecto,
        decimal ValorExonerado,
        decimal TotalIsc,
        decimal Igv,
        decimal TotalIcbper,
        decimal ImporteTotal,
        int Estado,
        IReadOnlyList<SaleItemDto> Items);

    public sealed record SaleItemDto(
        int IdDetalle,
        int IdArticulo,
        string Codigo,
        string Descripcion,
        string SiglaUnidad,
        decimal Cantidad,
        decimal PrecioUnitario,
        int TipoAfectacionIgv,
        decimal ValorVenta,
        decimal Descuento,
        decimal Isc,
        decimal Igv,
        decimal Icbper,
        decimal Subtotal);
}
