namespace DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Queries.GetSubmissionStatus
{
    public sealed record GetSubmissionStatusResponse(
        int IdVenta,
        int Estado,
        string? CodigoRespuesta,
        string? MensajeRespuesta,
        string? XmlHash,
        string? NumTicket,
        int Intentos,
        DateTime FechaEnvio,
        DateTime? FechaModificacion);
}
