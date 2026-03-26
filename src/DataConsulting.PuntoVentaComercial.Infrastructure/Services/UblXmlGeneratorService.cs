using System.Xml;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Settings;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Domain.Empresas;
using DataConsulting.PuntoVentaComercial.Domain.Ventas;
using Microsoft.Extensions.Options;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Services;

internal sealed class UblXmlGeneratorService(IOptions<SunatSettings> sunatOptions) : IUblXmlGeneratorService
{
    // ── Namespaces UBL 2.1 ───────────────────────────────────────────────────
    private const string NsCbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
    private const string NsCac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";

    private const string CurrencyPen = "PEN";
    private const string CurrencyUsd = "USD";
    private const string Fmt         = "F2";

    // ── Punto de entrada ─────────────────────────────────────────────────────

    public XmlDocument GenerarDocumento(Venta venta, Cliente cliente, EmpresaFirmante firmante, string codigoSunat)
    {
        string templatePath = sunatOptions.Value.RutaPlantillas;
        if (string.IsNullOrWhiteSpace(templatePath))
            throw new InvalidOperationException("Sunat:RutaPlantillas not configured.");

        string templateFile = Path.Combine(templatePath, "FacturaUbl2_1_SUNAT.xml");
        if (!File.Exists(templateFile))
            throw new FileNotFoundException($"XML template not found: {templateFile}");

        var doc = new XmlDocument();
        doc.Load(templateFile);

        string moneda        = venta.IdTipoMoneda == 2 ? CurrencyUsd : CurrencyPen;
        string docId         = $"{venta.NumSerieA}-{venta.NumeroDocumentoA}";
        string tipoOperacion = "0101"; // Catálogo 17: venta interna afecta IGV

        FillHeader(doc, venta, codigoSunat, moneda, docId, tipoOperacion);
        FillSignature(doc, firmante, docId);
        FillSupplier(doc, firmante);
        FillCustomer(doc, cliente, venta, codigoSunat);
        FillDiscounts(doc, venta, moneda);
        FillTaxTotal(doc, venta, moneda);
        FillMonetaryTotal(doc, venta, moneda);
        BuildInvoiceLines(doc, venta, moneda);

        return doc;
    }

    // ── Header ───────────────────────────────────────────────────────────────

    private static void FillHeader(
        XmlDocument doc, Venta venta, string codigoSunat,
        string moneda, string docId, string tipoOperacion)
    {
        SetText(doc, "cbc:ID",                   docId);
        SetText(doc, "cbc:IssueDate",             venta.FechaEmision.ToString("yyyy-MM-dd"));
        SetText(doc, "cbc:IssueTime",             venta.FechaEmision.ToString("HH:mm:ss"));
        SetText(doc, "cbc:DueDate",               venta.FechaEmision.ToString("yyyy-MM-dd"));
        SetText(doc, "cbc:ProfileID",             tipoOperacion);
        SetText(doc, "cbc:Note",                  ImporteEnLetras(venta.ImporteTotal, moneda));
        SetText(doc, "cbc:DocumentCurrencyCode",  moneda);

        var typeCode = doc.GetElementsByTagName("cbc:InvoiceTypeCode").Item(0);
        if (typeCode != null)
        {
            typeCode.InnerText = codigoSunat;
            SetAttr(typeCode, "listID", tipoOperacion);
        }

        var lineCount = doc.GetElementsByTagName("cbc:LineCountNumeric").Item(0);
        if (lineCount != null) lineCount.InnerText = venta.Detalles.Count.ToString();

        // Referencia a guía y doc adicional son opcionales; se eliminan del template base
        RemoveNode(doc, "cac:DespatchDocumentReference");
        RemoveNode(doc, "cac:AdditionalDocumentReference");
    }

    // ── Signature ────────────────────────────────────────────────────────────

    private static void FillSignature(XmlDocument doc, EmpresaFirmante firmante, string docId)
    {
        var sig = doc.GetElementsByTagName("cac:Signature").Item(0);
        if (sig is null) return;

        // cbc:ID
        var sigId = Child(sig, "ID");
        if (sigId != null) sigId.InnerText = docId;

        // cac:SignatoryParty > cac:PartyIdentification > cbc:ID
        var signatoryParty = Child(sig, "SignatoryParty");
        var partyIdNode = Child(Child(signatoryParty, "PartyIdentification"), "ID");
        if (partyIdNode != null) partyIdNode.InnerText = firmante.NumDocumento;

        // cac:SignatoryParty > cac:PartyName > cbc:Name
        var partyNameNode = Child(Child(signatoryParty, "PartyName"), "Name");
        if (partyNameNode != null) partyNameNode.InnerText = firmante.RazonSocial;

        // cac:DigitalSignatureAttachment > cac:ExternalReference > cbc:URI
        var uri = Child(Child(Child(sig, "DigitalSignatureAttachment"), "ExternalReference"), "URI");
        if (uri != null) uri.InnerText = $"{firmante.NumDocumento}-{docId}";
    }

    // ── Supplier (empresa emisora) ───────────────────────────────────────────

    private static void FillSupplier(XmlDocument doc, EmpresaFirmante firmante)
    {
        var party = doc.GetElementsByTagName("cac:AccountingSupplierParty").Item(0)
            ?.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.LocalName == "Party");
        if (party is null) return;

        // cac:PartyIdentification > cbc:ID  (schemeID="6" = RUC)
        var partyId = Child(Child(party, "PartyIdentification"), "ID");
        if (partyId != null)
        {
            partyId.InnerText = firmante.NumDocumento;
            SetAttr(partyId, "schemeID", "6");
        }

        // cac:PartyName > cbc:Name
        var partyName = Child(Child(party, "PartyName"), "Name");
        if (partyName != null) partyName.InnerText = firmante.RazonSocial;

        // cac:PartyLegalEntity > cbc:RegistrationName
        var regName = Child(Child(party, "PartyLegalEntity"), "RegistrationName");
        if (regName != null) regName.InnerText = firmante.RazonSocial;
    }

    // ── Customer (receptor) ──────────────────────────────────────────────────

    private static void FillCustomer(
        XmlDocument doc, Cliente cliente, Venta venta, string codigoSunat)
    {
        // Boleta (03): comprador anónimo si monto <= 700 PEN y no tiene documento
        bool anonimo = codigoSunat == "03"
            && venta.ImporteTotal <= 700m
            && string.IsNullOrWhiteSpace(cliente.NumDocumento);

        string numDoc = anonimo ? "0"  : (cliente.NumDocumento ?? "-");
        string scheme = anonimo ? "0"  : GetClienteSchemeId(cliente);
        string nombre = anonimo ? "-"  : cliente.Nombre;

        var party = doc.GetElementsByTagName("cac:AccountingCustomerParty").Item(0)
            ?.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.LocalName == "Party");
        if (party is null) return;

        // cac:PartyIdentification > cbc:ID
        var custId = Child(Child(party, "PartyIdentification"), "ID");
        if (custId != null)
        {
            custId.InnerText = numDoc;
            SetAttr(custId, "schemeID", scheme);
        }

        // cac:PartyLegalEntity > cbc:RegistrationName
        var regName = Child(Child(party, "PartyLegalEntity"), "RegistrationName");
        if (regName != null) regName.InnerText = nombre;
    }

    // ── AllowanceCharge (descuento global de cabecera) ───────────────────────

    private static void FillDiscounts(XmlDocument doc, Venta venta, string moneda)
    {
        var allowance = doc.GetElementsByTagName("cac:AllowanceCharge").Item(0);
        if (allowance is null) return;

        decimal descuento   = venta.ImporteDescuento + (venta.ImporteDescuentoGlobal ?? 0);
        decimal baseImporte = venta.ValorVenta + descuento;

        SetAmount(allowance, "Amount",     descuento,   moneda);
        SetAmount(allowance, "BaseAmount", baseImporte, moneda);
    }

    // ── TaxTotal (resumen IGV de cabecera) ───────────────────────────────────

    private static void FillTaxTotal(XmlDocument doc, Venta venta, string moneda)
    {
        var taxTotal = doc.GetElementsByTagName("cac:TaxTotal").Item(0);
        if (taxTotal is null) return;

        SetAmount(taxTotal, "TaxAmount", venta.Igv, moneda);

        var taxSub = taxTotal.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.LocalName == "TaxSubtotal");
        if (taxSub is null) return;

        SetAmount(taxSub, "TaxableAmount", venta.ValorVenta, moneda);
        SetAmount(taxSub, "TaxAmount",     venta.Igv,        moneda);
    }

    // ── LegalMonetaryTotal ───────────────────────────────────────────────────

    private static void FillMonetaryTotal(XmlDocument doc, Venta venta, string moneda)
    {
        var total = doc.GetElementsByTagName("cac:LegalMonetaryTotal").Item(0);
        if (total is null) return;

        decimal descuento = venta.ImporteDescuento + (venta.ImporteDescuentoGlobal ?? 0);

        SetAmount(total, "AllowanceTotalAmount", descuento,          moneda);
        SetAmount(total, "ChargeTotalAmount",    0m,                 moneda);
        SetAmount(total, "PayableAmount",        venta.ImporteTotal, moneda);
    }

    // ── InvoiceLines ─────────────────────────────────────────────────────────

    private static void BuildInvoiceLines(XmlDocument doc, Venta venta, string moneda)
    {
        // Eliminar líneas del template antes de insertar las reales
        doc.GetElementsByTagName("cac:InvoiceLine")
            .Cast<XmlNode>().ToList()
            .ForEach(n => n.ParentNode!.RemoveChild(n));

        int numero = 1;
        foreach (var detalle in venta.Detalles)
            doc.DocumentElement!.AppendChild(BuildLine(doc, detalle, numero++, moneda));
    }

    private static XmlElement BuildLine(
        XmlDocument doc, VentaDetalle detalle, int numero, string moneda)
    {
        decimal cantidad       = detalle.Cantidad ?? 1m;
        decimal precioUnitario = detalle.PrecioUnitario;
        decimal valorUnitario  = detalle.ValorUnitario ?? precioUnitario;
        decimal valorVenta     = detalle.ValorVenta;
        decimal descuento      = detalle.ImporteDescuento;
        bool    afectoIgv      = detalle.Igv && !detalle.FlagExonerado;
        decimal igvLinea       = afectoIgv ? Math.Round(valorVenta * 0.18m, 2) : 0m;
        string  taxCatId       = afectoIgv ? "S" : (detalle.FlagExonerado ? "E" : "O");
        string  exemptCode     = GetTaxExemptionCode(detalle.IdTipoAfectoIGV, detalle.FlagExonerado);

        var line = Cac(doc, "InvoiceLine");
        line.AppendChild(Cbc(doc, "ID", numero.ToString()));
        line.AppendChild(BuildQuantity(doc, cantidad));
        line.AppendChild(Amount(doc, "LineExtensionAmount", valorVenta, moneda));
        line.AppendChild(BuildPricingReference(doc, precioUnitario, moneda));
        line.AppendChild(BuildLineAllowance(doc, descuento, valorVenta, moneda));
        line.AppendChild(BuildLineTaxTotal(doc, igvLinea, valorVenta, taxCatId, exemptCode, moneda));
        line.AppendChild(BuildItem(doc, detalle));
        line.AppendChild(BuildPrice(doc, valorUnitario, moneda));
        return line;
    }

    private static XmlElement BuildPricingReference(XmlDocument doc, decimal precio, string moneda)
    {
        var pricingRef = Cac(doc, "PricingReference");
        var altPrice   = Cac(doc, "AlternativeConditionPrice");
        altPrice.AppendChild(Amount(doc, "PriceAmount", precio, moneda));
        altPrice.AppendChild(Cbc(doc, "PriceTypeCode", "01")); // precio de venta unitario
        pricingRef.AppendChild(altPrice);
        return pricingRef;
    }

    private static XmlElement BuildLineAllowance(
        XmlDocument doc, decimal descuento, decimal valorVenta, string moneda)
    {
        var allowance = Cac(doc, "AllowanceCharge");
        allowance.AppendChild(Cbc(doc, "ChargeIndicator",            "false"));
        allowance.AppendChild(Cbc(doc, "AllowanceChargeReasonCode",  "00"));
        allowance.AppendChild(Amount(doc, "Amount",     descuento,              moneda));
        allowance.AppendChild(Amount(doc, "BaseAmount", valorVenta + descuento, moneda));
        return allowance;
    }

    private static XmlElement BuildLineTaxTotal(
        XmlDocument doc, decimal igv, decimal baseImponible,
        string taxCatId, string exemptCode, string moneda)
    {
        var taxTotal    = Cac(doc, "TaxTotal");
        var taxSubtotal = Cac(doc, "TaxSubtotal");

        taxTotal.AppendChild(Amount(doc, "TaxAmount", igv, moneda));

        taxSubtotal.AppendChild(Amount(doc, "TaxableAmount", baseImponible, moneda));
        taxSubtotal.AppendChild(Amount(doc, "TaxAmount",     igv,           moneda));
        taxSubtotal.AppendChild(BuildTaxCategory(doc, taxCatId, exemptCode, igv > 0));
        taxTotal.AppendChild(taxSubtotal);
        return taxTotal;
    }

    private static XmlElement BuildTaxCategory(
        XmlDocument doc, string categoryId, string exemptCode, bool afecto)
    {
        var cat = Cac(doc, "TaxCategory");

        var id = doc.CreateElement("cbc", "ID", NsCbc);
        id.InnerText = categoryId;
        id.SetAttribute("schemeID",        "UN/ECE 5305");
        id.SetAttribute("schemeName",      "Tax Category Identifier");
        id.SetAttribute("schemeAgencyName","United Nations Economic Commission for Europe");
        cat.AppendChild(id);

        cat.AppendChild(Cbc(doc, "Percent", afecto ? "18.00" : "0.00"));

        var exempt = doc.CreateElement("cbc", "TaxExemptionReasonCode", NsCbc);
        exempt.InnerText = exemptCode;
        exempt.SetAttribute("listAgencyName", "PE:SUNAT");
        exempt.SetAttribute("listName",       "SUNAT:Codigo de Tipo de Afectación del IGV");
        exempt.SetAttribute("listURI",        "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07");
        cat.AppendChild(exempt);

        cat.AppendChild(BuildIgvTaxScheme(doc));
        return cat;
    }

    private static XmlElement BuildIgvTaxScheme(XmlDocument doc)
    {
        var scheme = Cac(doc, "TaxScheme");

        var id = doc.CreateElement("cbc", "ID", NsCbc);
        id.InnerText = "1000";
        id.SetAttribute("schemeID",        "UN/ECE 5153");
        id.SetAttribute("schemeName",      "Tax Scheme Identifier");
        id.SetAttribute("schemeAgencyName","United Nations Economic Commission for Europe");
        scheme.AppendChild(id);

        scheme.AppendChild(Cbc(doc, "Name",        "IGV"));
        scheme.AppendChild(Cbc(doc, "TaxTypeCode", "VAT"));
        return scheme;
    }

    private static XmlElement BuildItem(XmlDocument doc, VentaDetalle detalle)
    {
        var item = Cac(doc, "Item");
        item.AppendChild(Cbc(doc, "Description", detalle.DescripcionArticulo ?? "-"));

        if (detalle.IdArticulo.HasValue)
        {
            var sellersId = Cac(doc, "SellersItemIdentification");
            sellersId.AppendChild(Cbc(doc, "ID", detalle.IdArticulo.Value.ToString()));
            item.AppendChild(sellersId);
        }
        return item;
    }

    private static XmlElement BuildPrice(XmlDocument doc, decimal valorUnitario, string moneda)
    {
        var price = Cac(doc, "Price");
        price.AppendChild(Amount(doc, "PriceAmount", valorUnitario, moneda));
        return price;
    }

    private static XmlElement BuildQuantity(XmlDocument doc, decimal qty)
    {
        var el = doc.CreateElement("cbc", "InvoicedQuantity", NsCbc);
        el.InnerText = qty.ToString(Fmt);
        el.SetAttribute("unitCode",              "NIU");
        el.SetAttribute("unitCodeListID",         "UN/ECE rec 20");
        el.SetAttribute("unitCodeListAgencyName", "United Nations Economic Commission for Europe");
        return el;
    }

    // ── Helpers de template (nodos existentes) ───────────────────────────────

    /// <summary>Establece InnerText de la primera ocurrencia del tag en el documento.</summary>
    private static void SetText(XmlDocument doc, string tagName, string value)
    {
        var node = doc.GetElementsByTagName(tagName).Item(0);
        if (node != null) node.InnerText = value;
    }

    /// <summary>Actualiza el valor de un atributo en un nodo.</summary>
    private static void SetAttr(XmlNode node, string attrName, string value)
    {
        if (node.Attributes?[attrName] is XmlAttribute a) a.Value = value;
    }

    /// <summary>Actualiza InnerText y currencyID de un nodo amount hijo directo.</summary>
    private static void SetAmount(XmlNode parent, string localName, decimal amount, string currencyId)
    {
        var node = parent.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.LocalName == localName);
        if (node is null) return;
        node.InnerText = amount.ToString(Fmt);
        if (node.Attributes?["currencyID"] is XmlAttribute a) a.Value = currencyId;
    }

    /// <summary>Elimina la primera ocurrencia del tag del documento.</summary>
    private static void RemoveNode(XmlDocument doc, string tagName)
    {
        var node = doc.GetElementsByTagName(tagName).Item(0);
        node?.ParentNode?.RemoveChild(node);
    }

    /// <summary>Retorna el primer hijo con el LocalName indicado, o null.</summary>
    private static XmlNode? Child(XmlNode? parent, string localName) =>
        parent?.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.LocalName == localName);

    // ── Helpers de creación de nodos ─────────────────────────────────────────

    /// <summary>Crea un elemento cac: (CommonAggregateComponents).</summary>
    private static XmlElement Cac(XmlDocument doc, string localName) =>
        doc.CreateElement("cac", localName, NsCac);

    /// <summary>Crea un elemento cbc: (CommonBasicComponents) con texto.</summary>
    private static XmlElement Cbc(XmlDocument doc, string localName, string value)
    {
        var el = doc.CreateElement("cbc", localName, NsCbc);
        el.InnerText = value;
        return el;
    }

    /// <summary>Crea un elemento cbc: de importe monetario con atributo currencyID.</summary>
    private static XmlElement Amount(XmlDocument doc, string localName, decimal amount, string currencyId)
    {
        var el = doc.CreateElement("cbc", localName, NsCbc);
        el.InnerText = amount.ToString(Fmt);
        el.SetAttribute("currencyID", currencyId);
        return el;
    }

    // ── Catálogos SUNAT ──────────────────────────────────────────────────────

    /// <summary>Retorna el schemeID del documento de identidad del cliente (Catálogo 06).</summary>
    private static string GetClienteSchemeId(Cliente cliente) =>
        cliente.IdDocumentoIdentidad switch
        {
            6 => "6", // RUC
            1 => "1", // DNI
            4 => "4", // Carnet de extranjería
            7 => "7", // Pasaporte
            _ => "1"
        };

    /// <summary>Retorna el código de afectación al IGV (Catálogo 07).</summary>
    private static string GetTaxExemptionCode(int? idTipoAfectoIGV, bool flagExonerado) =>
        idTipoAfectoIGV switch
        {
            1  => "10", // Gravado – Operación Onerosa
            2  => "11", // Gravado – Retiro por premio
            3  => "12", // Gravado – Retiro por donación
            4  => "13", // Gravado – Retiro
            5  => "14", // Gravado – Retiro por publicidad
            6  => "15", // Gravado – Bonificaciones
            7  => "16", // Gravado – Retiro por entrega a trabajadores
            8  => "17", // Gravado – IVAP
            20 => "20", // Exonerado – Operación Onerosa
            21 => "21", // Exonerado – Transferencia Gratuita
            30 => "30", // Inafecto – Operación Onerosa
            31 => "31", // Inafecto – Retiro por Bonificación
            32 => "32", // Inafecto – Retiro
            33 => "33", // Inafecto – Retiro por Muestras Médicas
            34 => "34", // Inafecto – Retiro a título Gratuito
            35 => "35", // Inafecto – Retiro por Sustitución de Bien
            36 => "36", // Inafecto – Retiro por entrega a trabajadores
            37 => "37", // Inafecto – IVAP
            _  => flagExonerado ? "20" : "10"
        };

    // ── Importe en letras ────────────────────────────────────────────────────

    private static string ImporteEnLetras(decimal importe, string moneda)
    {
        string monedaNombre = moneda == CurrencyUsd ? "DÓLARES AMERICANOS" : "SOLES";
        long entero   = (long)Math.Floor(importe);
        int  centavos = (int)Math.Round((importe - Math.Floor(importe)) * 100);
        return $"{NumeroEnLetras(entero)} Y {centavos:D2}/100 {monedaNombre}";
    }

    private static string NumeroEnLetras(long numero)
    {
        if (numero == 0) return "CERO";
        if (numero < 0)  return "MENOS " + NumeroEnLetras(-numero);

        string[] unidades = ["", "UNO", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE",
            "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISÉIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE"];
        string[] decenas  = ["", "DIEZ", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA"];
        string[] centenas = ["", "CIEN", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS",
            "SEISCIENTOS", "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS"];

        if (numero < 20)    return unidades[numero];
        if (numero < 100)   return decenas[numero / 10] + (numero % 10 != 0 ? " Y " + unidades[numero % 10] : "");
        if (numero < 1000)  return numero == 100 ? "CIEN" : centenas[numero / 100] + (numero % 100 != 0 ? " " + NumeroEnLetras(numero % 100) : "");
        if (numero < 2000)  return "MIL" + (numero % 1000 != 0 ? " " + NumeroEnLetras(numero % 1000) : "");
        if (numero < 1_000_000)
        {
            string miles = NumeroEnLetras(numero / 1000) + " MIL";
            return miles + (numero % 1000 != 0 ? " " + NumeroEnLetras(numero % 1000) : "");
        }
        if (numero < 2_000_000) return "UN MILLÓN" + (numero % 1_000_000 != 0 ? " " + NumeroEnLetras(numero % 1_000_000) : "");
        {
            string millones = NumeroEnLetras(numero / 1_000_000) + " MILLONES";
            return millones + (numero % 1_000_000 != 0 ? " " + NumeroEnLetras(numero % 1_000_000) : "");
        }
    }
}
