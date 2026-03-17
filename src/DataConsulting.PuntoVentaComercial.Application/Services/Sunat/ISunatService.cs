using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Services.Sunat
{
    /// <summary>
    /// Abstracción del servicio de facturación electrónica SUNAT.
    /// La implementación real genera XML UBL 2.1, firma digitalmente y llama
    /// al endpoint SUNAT (SendBill/SendSummary).
    /// </summary>
    public interface ISunatService
    {
        /// <summary>
        /// Genera XML UBL 2.1, firma con certificado X.509, envía a SUNAT y retorna el resultado.
        /// </summary>
        Task<SunatSubmissionResult> SubmitInvoiceAsync(
            SunatInvoiceData data,
            CancellationToken cancellationToken = default);
    }

    public sealed record SunatInvoiceData(
        // Emisor
        string RucEmisor,
        string RazonSocialEmisor,
        string DireccionEmisor,
        // Documento
        EDocumento TipoDocumento,
        string NumSerie,
        long Correlativo,
        DateTime FechaEmision,
        // Receptor
        int IdDocumentoIdentidadCliente,
        string NumDocumentoCliente,
        string NombreCliente,
        string DireccionCliente,
        // Totales
        decimal ValorAfecto,
        decimal ValorInafecto,
        decimal ValorExonerado,
        decimal ValorGratuito,
        decimal TotalIsc,
        decimal Igv,
        decimal TotalIcbper,
        decimal ImporteTotal,
        // Ítems
        IReadOnlyList<SunatInvoiceItemData> Items);

    public sealed record SunatInvoiceItemData(
        string Codigo,
        string Descripcion,
        /// <summary>Código de unidad de medida UBL (ej. "NIU" = Unidad, "KGM" = Kilogramo).</summary>
        string UnidadMedidaUbl,
        decimal Cantidad,
        ETipoAfectacionIgv TipoAfectacionIgv,
        decimal ValorUnitario,
        decimal ValorVenta,
        decimal Descuento,
        decimal Isc,
        decimal Igv,
        decimal Icbper,
        decimal Subtotal);

    public sealed record SunatSubmissionResult(
        bool Accepted,
        string CodigoRespuesta,
        string MensajeRespuesta,
        string? XmlHash,
        string? CdrXml,
        /// <summary>Ticket para consulta asíncrona (solo en sendSummary).</summary>
        string? NumTicket);
}
