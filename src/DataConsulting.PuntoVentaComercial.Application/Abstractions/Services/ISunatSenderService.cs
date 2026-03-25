namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

public sealed record SunatSendResponse(
    bool Accepted,
    string CodigoRespuesta,
    string Descripcion,
    byte[]? CdrZip);

public interface ISunatSenderService
{
    Task<SunatSendResponse> SendBillAsync(
        string zipFileName,
        byte[] zipContent,
        string rucUsuario,
        string clave,
        bool usePruebas,
        CancellationToken ct);
}
