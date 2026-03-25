using System.Xml;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Domain.Empresas;
using DataConsulting.PuntoVentaComercial.Domain.Ventas;

namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

public interface IUblXmlGeneratorService
{
    XmlDocument GenerarFactura(Venta venta, Cliente cliente, EmpresaFirmante firmante, string codigoSunat);
}
