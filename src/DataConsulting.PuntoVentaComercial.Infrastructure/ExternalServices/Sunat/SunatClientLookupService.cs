using DataConsulting.PuntoVentaComercial.Application.Services.Sunat;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.ExternalServices.Sunat
{
    /// <summary>
    /// Stub de consulta SUNAT por RUC.
    /// En el legacy esto lo hace trans.BuscarRuc(ruc, usuario, pass) via WCF SOAP.
    /// La integración real se implementa en la fase B9.
    /// Por ahora retorna null para que el handler devuelva ClientErrors.RucNoEncontradoEnSunat.
    /// </summary>
    internal sealed class SunatClientLookupService : ISunatClientLookupService
    {
        public Task<SunatRucInfo?> LookupByRucAsync(string ruc, CancellationToken cancellationToken = default)
        {
            // TODO (B9): Implementar llamada al servicio WCF/REST de SUNAT.
            return Task.FromResult<SunatRucInfo?>(null);
        }
    }
}
