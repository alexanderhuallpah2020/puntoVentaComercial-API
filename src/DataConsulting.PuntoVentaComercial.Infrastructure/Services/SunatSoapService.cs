using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Services;

internal sealed class SunatSoapService(IHttpClientFactory httpClientFactory) : ISunatSenderService
{
    private const string UrlBeta = "https://e-beta.sunat.gob.pe/ol-ti-itcpfegem-beta/billService";
    private const string UrlProd = "https://e-factura.sunat.gob.pe/ol-ti-itcpfegem/billService";

    public async Task<SunatSendResponse> SendBillAsync(
        string zipFileName, byte[] zipContent,
        string rucUsuario, string clave,
        bool usePruebas, CancellationToken ct)
    {
        string url = usePruebas ? UrlBeta : UrlProd;
        string base64Zip = Convert.ToBase64String(zipContent);

        string soapEnvelope = BuildSoapEnvelope(rucUsuario, clave, zipFileName, base64Zip);

        var client = httpClientFactory.CreateClient("sunat");
        var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", "\"\"");

        HttpResponseMessage httpResponse;
        try
        {
            httpResponse = await client.PostAsync(url, content, ct);
        }
        catch (Exception ex)
        {
            return new SunatSendResponse(false, "ERROR_CONEXION", ex.Message, null);
        }

        string responseBody = await httpResponse.Content.ReadAsStringAsync(ct);
        return ParseSoapResponse(responseBody);
    }

    private static string BuildSoapEnvelope(string rucUsuario, string clave, string fileName, string base64Content) => $"""
        <soapenv:Envelope
            xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
            xmlns:ser="http://service.sunat.gob.pe"
            xmlns:wsse="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd">
          <soapenv:Header>
            <wsse:Security>
              <wsse:UsernameToken>
                <wsse:Username>{SecurityEncode(rucUsuario)}</wsse:Username>
                <wsse:Password>{SecurityEncode(clave)}</wsse:Password>
              </wsse:UsernameToken>
            </wsse:Security>
          </soapenv:Header>
          <soapenv:Body>
            <ser:sendBill>
              <fileName>{SecurityEncode(fileName)}</fileName>
              <contentFile>{base64Content}</contentFile>
            </ser:sendBill>
          </soapenv:Body>
        </soapenv:Envelope>
        """;

    private static string SecurityEncode(string value) =>
        value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    private static SunatSendResponse ParseSoapResponse(string responseBody)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(responseBody);

            // Check for SOAP fault
            var fault = doc.GetElementsByTagName("faultcode");
            if (fault.Count > 0)
            {
                var faultString = doc.GetElementsByTagName("faultstring").Item(0)?.InnerText ?? "Error SOAP";
                return new SunatSendResponse(false, "FAULT", faultString, null);
            }

            // Extract applicationResponse (CDR zip in base64)
            var appResponse = doc.GetElementsByTagName("applicationResponse").Item(0)?.InnerText;
            if (string.IsNullOrWhiteSpace(appResponse))
                return new SunatSendResponse(false, "SIN_RESPUESTA", "SUNAT no devolvió CDR.", null);

            byte[] cdrZip = Convert.FromBase64String(appResponse);
            var (codigo, descripcion) = ParseCdr(cdrZip);

            bool accepted = codigo == "0";
            return new SunatSendResponse(accepted, codigo, descripcion, cdrZip);
        }
        catch (Exception ex)
        {
            return new SunatSendResponse(false, "ERROR_PARSE", ex.Message, null);
        }
    }

    private static (string Codigo, string Descripcion) ParseCdr(byte[] cdrZip)
    {
        try
        {
            using var ms = new System.IO.MemoryStream(cdrZip);
            using var archive = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                if (!entry.Name.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) continue;

                using var stream = entry.Open();
                var cdrDoc = new XmlDocument();
                cdrDoc.Load(stream);

                var nsMgr = new XmlNamespaceManager(cdrDoc.NameTable);
                nsMgr.AddNamespace("ar", "urn:oasis:names:specification:ubl:schema:xsd:ApplicationResponse-2");
                nsMgr.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                nsMgr.AddNamespace("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

                var responseCode = cdrDoc.SelectSingleNode(
                    "//cac:DocumentResponse/cac:Response/cbc:ResponseCode", nsMgr)?.InnerText ?? "?";
                var description = cdrDoc.SelectSingleNode(
                    "//cac:DocumentResponse/cac:Response/cbc:Description", nsMgr)?.InnerText ?? "";

                return (responseCode, description);
            }
        }
        catch { /* si el CDR falla de parsear, devolvemos código desconocido */ }

        return ("?", "No se pudo leer el CDR.");
    }
}
