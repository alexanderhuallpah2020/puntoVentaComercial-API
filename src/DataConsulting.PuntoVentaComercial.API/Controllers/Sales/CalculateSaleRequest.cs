using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Sales
{
    public sealed record CalculateSaleRequest(
        EDocumentoIdentidad TipoDocumentoCliente,
        List<CalculateSaleItemRequest> Items,
        decimal DescuentoGlobal,
        EFormaPago FormaPago);

    public sealed record CalculateSaleItemRequest(
        string? ProductoId,
        decimal Cantidad,
        decimal PrecioUnitario,
        ETipoAfectacionIgv TipoAfectacionIgv,
        ETipoIsc? TipoIsc,
        decimal? TasaIsc,
        decimal? MontoFijoIsc,
        bool EsBolsaPlastica);
}
