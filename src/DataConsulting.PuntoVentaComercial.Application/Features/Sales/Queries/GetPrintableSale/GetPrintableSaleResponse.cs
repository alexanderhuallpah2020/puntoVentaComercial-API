using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetPrintableSale
{
    public sealed record GetPrintableSaleResponse(
        // Datos de empresa
        string NombreEmpresa,
        string RucEmpresa,
        string DireccionEmpresa,
        // Documento
        int IdVenta,
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

    public sealed record PrintItemDto(
        string Codigo,
        string Descripcion,
        string SiglaUnidad,
        decimal Cantidad,
        decimal PrecioUnitario,
        decimal Descuento,
        decimal Subtotal);
}
