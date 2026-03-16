using DataConsulting.PuntoVentaComercial.Application.Services.Sunat;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.ExternalServices.Sunat
{
    /// <summary>
    /// Implementación stub del servicio de consulta de RUC en SUNAT.
    /// TODO B9: Integrar con WebfacturaDCPSoapClient o API REST SUNAT.
    /// El legacy usa trans.BuscarRuc(ruc, usuario, pass) vía WCF (WebSerial.WebfacturaDCPSoapClient).
    /// </summary>
    internal sealed class SunatClientLookupService : ISunatClientLookupService
    {
        public Task<RucInfo?> LookupByRucAsync(string ruc, CancellationToken ct = default)
        {
            // Retorna null → el handler interpretará como "no encontrado en SUNAT"
            return Task.FromResult<RucInfo?>(null);
        }
    }
}
