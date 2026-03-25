using System.Xml;

namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

public interface IXmlSignerService
{
    void Sign(XmlDocument xmlDoc, string certPath, string certPassword);
}
