using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CalculateSale
{
    public sealed record CalculateSaleResponse(
        EDocumento TipoDocumento,
        decimal ValorAfecto,
        decimal ValorInafecto,
        decimal ValorExonerado,
        decimal ValorGratuito,
        decimal DescuentoGlobal,
        decimal BaseImponible,
        decimal Igv,
        decimal Isc,
        decimal Icbper,
        decimal Total,
        List<CalculateSaleItemResponse> Items);

    public sealed record CalculateSaleItemResponse(
        string? ProductoId,
        decimal Cantidad,
        decimal PrecioUnitario,
        ETipoAfectacionIgv TipoAfectacionIgv,
        decimal ValorItem,
        decimal DescuentoItem,
        decimal ValorAfectoItem,
        decimal IscItem,
        decimal IcbperItem,
        decimal IgvItem,
        decimal TotalItem);
}
