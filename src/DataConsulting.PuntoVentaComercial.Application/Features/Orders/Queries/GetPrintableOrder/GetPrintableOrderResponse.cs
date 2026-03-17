using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetPrintableSale;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetPrintableOrder
{
    public sealed record GetPrintableOrderResponse(
        // Datos de empresa
        string NombreEmpresa,
        string RucEmpresa,
        string DireccionEmpresa,
        // Documento
        int IdPedido,
        EDocumento TipoDocumento,
        string NumeroFormateado,
        DateTime FechaEmision,
        // Cliente
        int IdDocumentoIdentidad,
        string NombreCliente,
        string NumDocumentoCliente,
        string DireccionCliente,
        // Condiciones
        bool FlagIGV,
        ETipoMoneda TipoMoneda,
        decimal DescuentoGlobal,
        // Totales
        decimal ValorAfecto,
        decimal ValorInafecto,
        decimal ValorExonerado,
        decimal ValorGratuito,
        decimal TotalIsc,
        decimal Igv,
        decimal TotalIcbper,
        decimal ImporteTotal,
        string? Observaciones,
        IReadOnlyList<PrintItemDto> Items);
}
