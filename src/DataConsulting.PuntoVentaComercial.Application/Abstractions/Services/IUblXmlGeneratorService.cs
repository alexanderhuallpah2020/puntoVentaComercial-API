using System.Xml;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Domain.Empresas;
using DataConsulting.PuntoVentaComercial.Domain.Ventas;

namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

public interface IUblXmlGeneratorService
{
    /// <summary>Genera XML UBL 2.1 para Factura (01) o Boleta (03).</summary>
    XmlDocument GenerarDocumento(Venta venta, Cliente cliente, EmpresaFirmante firmante, string codigoSunat);

    /// <summary>Genera XML UBL 2.1 para Nota de Crédito (07).</summary>
    XmlDocument GenerarNotaCredito(Venta venta, Cliente cliente, EmpresaFirmante firmante);

    /// <summary>Genera XML UBL 2.1 para Nota de Débito (08).</summary>
    XmlDocument GenerarNotaDebito(Venta venta, Cliente cliente, EmpresaFirmante firmante);
}
