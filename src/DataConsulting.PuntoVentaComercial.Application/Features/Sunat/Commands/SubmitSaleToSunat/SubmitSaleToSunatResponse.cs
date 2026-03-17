namespace DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Commands.SubmitSaleToSunat
{
    public sealed record SubmitSaleToSunatResponse(
        int IdVenta,
        bool Accepted,
        string CodigoRespuesta,
        string MensajeRespuesta,
        string? XmlHash,
        string? NumTicket);
}
