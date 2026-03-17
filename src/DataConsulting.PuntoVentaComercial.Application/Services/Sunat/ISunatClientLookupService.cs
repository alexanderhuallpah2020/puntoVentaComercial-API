namespace DataConsulting.PuntoVentaComercial.Application.Services.Sunat
{
    /// <summary>
    /// Abstracción para consultar datos de un contribuyente en SUNAT por RUC.
    /// En el legacy esto lo hace: trans.BuscarRuc(ruc, usuario, pass) via WCF SOAP.
    /// La implementación real vive en Infrastructure.
    /// </summary>
    public interface ISunatClientLookupService
    {
        Task<SunatRucInfo?> LookupByRucAsync(string ruc, CancellationToken cancellationToken = default);
    }

    public sealed record SunatRucInfo(
        string Ruc,
        string RazonSocial,
        string Direccion,
        string Ubigeo,
        string Departamento,
        string Provincia,
        string Distrito,
        bool Activo);
}
