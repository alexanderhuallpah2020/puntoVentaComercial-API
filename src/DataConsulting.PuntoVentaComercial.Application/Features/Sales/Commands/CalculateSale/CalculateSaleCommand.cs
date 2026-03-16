using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CalculateSale
{
    public sealed record CalculateSaleCommand(
        EDocumentoIdentidad TipoDocumentoCliente,
        List<CalculateSaleItemCommand> Items,
        decimal DescuentoGlobal,
        EFormaPago FormaPago) : ICommand<CalculateSaleResponse>;

    public sealed record CalculateSaleItemCommand(
        string? ProductoId,
        decimal Cantidad,
        decimal PrecioUnitario,
        ETipoAfectacionIgv TipoAfectacionIgv,
        ETipoIsc? TipoIsc,
        decimal? TasaIsc,
        decimal? MontoFijoIsc,
        bool EsBolsaPlastica);
}
