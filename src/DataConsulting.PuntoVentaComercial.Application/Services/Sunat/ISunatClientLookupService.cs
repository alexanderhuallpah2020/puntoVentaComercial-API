namespace DataConsulting.PuntoVentaComercial.Application.Services.Sunat
{
    public record RucInfo(
        string RazonSocial,
        string Direccion,
        string Estado,
        string TipoContribuyente);

    /// <summary>
    /// Abstracción para consultar datos de un contribuyente en SUNAT por número de RUC.
    /// La implementación real se integrará en la Fase B9 con el servicio WCF/REST de SUNAT.
    /// </summary>
    public interface ISunatClientLookupService
    {
        Task<RucInfo?> LookupByRucAsync(string ruc, CancellationToken ct = default);
    }
}
