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
    private const string CurrencyPen = "PEN";
    private const string CurrencyUsd = "USD";
    private const string Fmt = "F2";

    public XmlDocument GenerarFactura(Venta venta, Cliente cliente, EmpresaFirmante firmante, string codigoSunat)
    {
        string templatePath = sunatOptions.Value.RutaPlantillas;
        if (string.IsNullOrWhiteSpace(templatePath))
            throw new InvalidOperationException("Sunat:RutaPlantillas not configured.");

        string templateFile = Path.Combine(templatePath, "FacturaUbl2_1_SUNAT.xml");
        if (!File.Exists(templateFile))
            throw new FileNotFoundException($"XML template not found: {templateFile}");

        var doc = new XmlDocument { PreserveWhitespace = true };
        doc.Load(templateFile);

        string moneda = venta.IdTipoMoneda == 2 ? CurrencyUsd : CurrencyPen;
        string docId = $"{venta.NumSerieA}-{venta.NumeroDocumentoA}";

        // Determine operation type (catalog 17): 0101=venta interna afecta IGV
        string tipoOperacion = "0101";

        // --- Header fields ---
        SetInnerText(doc, "cbc:ID", docId);
        SetInnerText(doc, "cbc:IssueDate", venta.FechaEmision.ToString("yyyy-MM-dd"));
        SetInnerText(doc, "cbc:IssueTime", venta.FechaEmision.ToString("HH:mm:ss"));

        // DueDate — same as issue date for immediate payment
        var dueDate = doc.GetElementsByTagName("cbc:DueDate").Item(0);
        if (dueDate != null) dueDate.InnerText = venta.FechaEmision.ToString("yyyy-MM-dd");

        var typeCode = doc.GetElementsByTagName("cbc:InvoiceTypeCode").Item(0);
        if (typeCode != null)
        {
            typeCode.InnerText = codigoSunat;
            if (typeCode.Attributes?["listID"] is XmlAttribute listIdAttr)
                listIdAttr.Value = tipoOperacion;
        }

        var profileId = doc.GetElementsByTagName("cbc:ProfileID").Item(0);
        if (profileId != null) profileId.InnerText = tipoOperacion;

        // Amount in words note
        var note = doc.GetElementsByTagName("cbc:Note").Item(0);
        if (note != null) note.InnerText = ImporteEnLetras(venta.ImporteTotal, moneda);

        SetInnerText(doc, "cbc:DocumentCurrencyCode", moneda);

        var lineCount = doc.GetElementsByTagName("cbc:LineCountNumeric").Item(0);
        if (lineCount != null) lineCount.InnerText = venta.Detalles.Count.ToString();

        // Remove dispatch/additional document reference nodes (optional for basic invoice)
        RemoveNode(doc, "cac:DespatchDocumentReference");
        RemoveNode(doc, "cac:AdditionalDocumentReference");

        // --- Signature block ---
        var sigBlock = doc.GetElementsByTagName("cac:Signature").Item(0);
        if (sigBlock != null)
        {
            sigBlock.ChildNodes.Item(0)!.InnerText = docId;                                          // cbc:ID
            sigBlock.ChildNodes.Item(1)!.ChildNodes.Item(0)!.ChildNodes.Item(0)!.InnerText = firmante.NumDocumento; // cac:PartyIdentification/cbc:ID
            sigBlock.ChildNodes.Item(1)!.ChildNodes.Item(1)!.ChildNodes.Item(0)!.InnerText = firmante.RazonSocial;  // cac:PartyName/cbc:Name
            sigBlock.ChildNodes.Item(2)!.ChildNodes.Item(0)!.ChildNodes.Item(0)!.InnerText = $"{firmante.NumDocumento}-{docId}"; // URI
        }

        // --- Supplier (empresa emisora) ---
        var supplier = doc.GetElementsByTagName("cac:AccountingSupplierParty").Item(0)?.ChildNodes.Item(0);
        if (supplier != null)
        {
            var partyId = supplier.ChildNodes.Item(0)?.ChildNodes.Item(0); // cac:PartyIdentification/cbc:ID
            if (partyId != null)
            {
                partyId.InnerText = firmante.NumDocumento;
                if (partyId.Attributes?["schemeID"] is XmlAttribute s) s.Value = "6";
            }
            var partyName = supplier.ChildNodes.Item(1)?.ChildNodes.Item(0); // cac:PartyName/cbc:Name
            if (partyName != null) partyName.InnerText = firmante.RazonSocial;
            var legalName = supplier.ChildNodes.Item(2)?.ChildNodes.Item(0); // cac:PartyLegalEntity/cbc:RegistrationName
            if (legalName != null) legalName.InnerText = firmante.RazonSocial;
        }

        // --- Customer ---
        var customer = doc.GetElementsByTagName("cac:AccountingCustomerParty").Item(0)?.ChildNodes.Item(0);
        if (customer != null)
        {
            string schemeId = GetClienteSchemeId(cliente);
            var custId = customer.ChildNodes.Item(0)?.ChildNodes.Item(0); // cac:PartyIdentification/cbc:ID
            if (custId != null)
            {
                custId.InnerText = cliente.NumDocumento ?? "-";
                if (custId.Attributes?["schemeID"] is XmlAttribute s) s.Value = schemeId;
            }
            var legalEntity = customer.ChildNodes.Item(1); // cac:PartyLegalEntity
            if (legalEntity != null)
            {
                var regName = legalEntity.ChildNodes.Item(0);
                if (regName != null) regName.InnerText = cliente.Nombre;
            }
        }

        // --- AllowanceCharge (header discount) ---
        var allowance = doc.GetElementsByTagName("cac:AllowanceCharge").Item(0);
        if (allowance != null)
        {
            decimal descuento = venta.ImporteDescuento + (venta.ImporteDescuentoGlobal ?? 0);
            decimal baseDescuento = venta.ValorVenta + descuento;
            SetAmountNode(allowance, "cbc:Amount", descuento, moneda);
            SetAmountNode(allowance, "cbc:BaseAmount", baseDescuento, moneda);
        }

        // --- TaxTotal ---
        decimal igvTotal = venta.Igv;
        decimal taxableBase = venta.ValorVenta;

        var taxTotal = doc.GetElementsByTagName("cac:TaxTotal").Item(0);
        if (taxTotal != null)
        {
            SetAmountNode(taxTotal, "cbc:TaxAmount", igvTotal, moneda);
            var taxSub = taxTotal.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.LocalName == "TaxSubtotal");
            if (taxSub != null)
            {
                SetAmountNode(taxSub, "cbc:TaxableAmount", taxableBase, moneda);
                SetAmountNode(taxSub, "cbc:TaxAmount", igvTotal, moneda);
            }
        }

        // --- LegalMonetaryTotal ---
        var monetaryTotal = doc.GetElementsByTagName("cac:LegalMonetaryTotal").Item(0);
        if (monetaryTotal != null)
        {
            decimal descuento = venta.ImporteDescuento + (venta.ImporteDescuentoGlobal ?? 0);
            SetAmountNode(monetaryTotal, "cbc:AllowanceTotalAmount", descuento, moneda);
            SetAmountNode(monetaryTotal, "cbc:ChargeTotalAmount", 0m, moneda);
            SetAmountNode(monetaryTotal, "cbc:PayableAmount", venta.ImporteTotal, moneda);
        }

        // --- InvoiceLines ---
        BuildInvoiceLines(doc, venta, moneda);

        return doc;
    }

    private static void BuildInvoiceLines(XmlDocument doc, Venta venta, string moneda)
    {
        // Remove existing template InvoiceLines
        var existingLines = doc.GetElementsByTagName("cac:InvoiceLine");
        var linesToRemove = existingLines.Cast<XmlNode>().ToList();
        foreach (var line in linesToRemove)
            line.ParentNode!.RemoveChild(line);

        var root = doc.DocumentElement!;

        int lineNumber = 1;
        foreach (var detalle in venta.Detalles)
        {
            decimal cantidad = detalle.Cantidad ?? 1m;
            decimal precioUnitario = detalle.PrecioUnitario;
            decimal valorUnitario = detalle.ValorUnitario ?? precioUnitario;
            decimal lineExtension = detalle.ValorVenta;
            decimal lineDescuento = detalle.ImporteDescuento;

            // IGV per line: if afecto, ValorVenta * 0.18
            bool afectoIgv = detalle.Igv && !detalle.FlagExonerado;
            decimal igvLine = afectoIgv ? Math.Round(lineExtension * 0.18m, 2) : 0m;

            // Tax exemption reason code (catalog 07)
            string taxExemptionCode = GetTaxExemptionCode(detalle.IdTipoAfectoIGV, detalle.FlagExonerado);
            string taxCategoryId = afectoIgv ? "S" : (detalle.FlagExonerado ? "E" : "O");

            var lineNode = doc.CreateElement("cac", "InvoiceLine",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");

            lineNode.AppendChild(CreateCbc(doc, "ID", lineNumber.ToString()));
            lineNode.AppendChild(CreateQuantityNode(doc, cantidad));
            lineNode.AppendChild(CreateAmountNode(doc, "cbc", "LineExtensionAmount", lineExtension, moneda,
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"));

            // PricingReference
            var pricingRef = doc.CreateElement("cac", "PricingReference",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            var altCondPrice = doc.CreateElement("cac", "AlternativeConditionPrice",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            altCondPrice.AppendChild(CreateAmountNode(doc, "cbc", "PriceAmount", precioUnitario, moneda,
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"));
            altCondPrice.AppendChild(CreateCbc(doc, "PriceTypeCode", "01"));
            pricingRef.AppendChild(altCondPrice);
            lineNode.AppendChild(pricingRef);

            // AllowanceCharge for line discount
            var lineAllowance = doc.CreateElement("cac", "AllowanceCharge",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            lineAllowance.AppendChild(CreateCbc(doc, "ChargeIndicator", "false"));
            lineAllowance.AppendChild(CreateCbc(doc, "AllowanceChargeReasonCode", "00"));
            lineAllowance.AppendChild(CreateAmountNode(doc, "cbc", "Amount", lineDescuento, moneda,
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"));
            lineAllowance.AppendChild(CreateAmountNode(doc, "cbc", "BaseAmount",
                lineExtension + lineDescuento, moneda,
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"));
            lineNode.AppendChild(lineAllowance);

            // TaxTotal for line
            var lineTaxTotal = doc.CreateElement("cac", "TaxTotal",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            lineTaxTotal.AppendChild(CreateAmountNode(doc, "cbc", "TaxAmount", igvLine, moneda,
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"));

            var taxSubtotal = doc.CreateElement("cac", "TaxSubtotal",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            taxSubtotal.AppendChild(CreateAmountNode(doc, "cbc", "TaxableAmount", lineExtension, moneda,
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"));
            taxSubtotal.AppendChild(CreateAmountNode(doc, "cbc", "TaxAmount", igvLine, moneda,
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"));

            var taxCategory = doc.CreateElement("cac", "TaxCategory",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            var catId = doc.CreateElement("cbc", "ID",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            catId.InnerText = taxCategoryId;
            catId.SetAttribute("schemeID", "UN/ECE 5305");
            catId.SetAttribute("schemeName", "Tax Category Identifier");
            catId.SetAttribute("schemeAgencyName", "United Nations Economic Commission for Europe");
            taxCategory.AppendChild(catId);

            var pct = doc.CreateElement("cbc", "Percent",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            pct.InnerText = afectoIgv ? "18.00" : "0.00";
            taxCategory.AppendChild(pct);

            var exemptCode = doc.CreateElement("cbc", "TaxExemptionReasonCode",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            exemptCode.InnerText = taxExemptionCode;
            exemptCode.SetAttribute("listAgencyName", "PE:SUNAT");
            exemptCode.SetAttribute("listName", "SUNAT:Codigo de Tipo de Afectación del IGV");
            exemptCode.SetAttribute("listURI", "urn:pe:gob:sunat:cpe:see:gem:catalogos:catalogo07");
            taxCategory.AppendChild(exemptCode);

            var taxScheme = doc.CreateElement("cac", "TaxScheme",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            var tsId = doc.CreateElement("cbc", "ID",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            tsId.InnerText = "1000";
            tsId.SetAttribute("schemeID", "UN/ECE 5153");
            tsId.SetAttribute("schemeName", "Tax Scheme Identifier");
            tsId.SetAttribute("schemeAgencyName", "United Nations Economic Commission for Europe");
            taxScheme.AppendChild(tsId);
            taxScheme.AppendChild(CreateCbc(doc, "Name", "IGV"));
            taxScheme.AppendChild(CreateCbc(doc, "TaxTypeCode", "VAT"));
            taxCategory.AppendChild(taxScheme);
            taxSubtotal.AppendChild(taxCategory);
            lineTaxTotal.AppendChild(taxSubtotal);
            lineNode.AppendChild(lineTaxTotal);

            // Item
            var item = doc.CreateElement("cac", "Item",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            item.AppendChild(CreateCbc(doc, "Description", detalle.DescripcionArticulo ?? "-"));
            if (detalle.IdArticulo.HasValue)
            {
                var sellersId = doc.CreateElement("cac", "SellersItemIdentification",
                    "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                sellersId.AppendChild(CreateCbc(doc, "ID", detalle.IdArticulo.Value.ToString()));
                item.AppendChild(sellersId);
            }
            lineNode.AppendChild(item);

            // Price (valor unitario sin IGV)
            var price = doc.CreateElement("cac", "Price",
                "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            price.AppendChild(CreateAmountNode(doc, "cbc", "PriceAmount", valorUnitario, moneda,
                "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"));
            lineNode.AppendChild(price);

            root.AppendChild(lineNode);
            lineNumber++;
        }
    }

    // --- XML helpers ---

    private static void SetInnerText(XmlDocument doc, string tagName, string value)
    {
        var node = doc.GetElementsByTagName(tagName).Item(0);
        if (node != null) node.InnerText = value;
    }

    private static void SetAmountNode(XmlNode parent, string localName, decimal amount, string currencyId)
    {
        var node = parent.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.LocalName == localName);
        if (node != null)
        {
            node.InnerText = amount.ToString(Fmt);
            if (node.Attributes?["currencyID"] is XmlAttribute a) a.Value = currencyId;
        }
    }

    private static void RemoveNode(XmlDocument doc, string tagName)
    {
        var node = doc.GetElementsByTagName(tagName).Item(0);
        node?.ParentNode?.RemoveChild(node);
    }

    private static XmlElement CreateCbc(XmlDocument doc, string localName, string value)
    {
        const string ns = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        var el = doc.CreateElement("cbc", localName, ns);
        el.InnerText = value;
        return el;
    }

    private static XmlElement CreateAmountNode(XmlDocument doc,
        string prefix, string localName, decimal amount, string currencyId, string ns)
    {
        var el = doc.CreateElement(prefix, localName, ns);
        el.InnerText = amount.ToString(Fmt);
        el.SetAttribute("currencyID", currencyId);
        return el;
    }

    private static XmlElement CreateQuantityNode(XmlDocument doc, decimal qty)
    {
        const string ns = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        var el = doc.CreateElement("cbc", "InvoicedQuantity", ns);
        el.InnerText = qty.ToString(Fmt);
        el.SetAttribute("unitCode", "NIU");
        el.SetAttribute("unitCodeListID", "UN/ECE rec 20");
        el.SetAttribute("unitCodeListAgencyName", "United Nations Economic Commission for Europe");
        return el;
    }

    private static string GetClienteSchemeId(Cliente cliente)
    {
        // idDocumentoIdentidad: 1=DNI, 6=RUC (SUNAT catalog 06)
        return cliente.IdDocumentoIdentidad switch
        {
            6 => "6",
            1 => "1",
            4 => "4", // carnet de extranjería
            7 => "7", // pasaporte
            _ => "1"
        };
    }

    private static string GetTaxExemptionCode(int? idTipoAfectoIGV, bool flagExonerado)
    {
        // Catalog 07: 10=Gravado-IGV, 20=Exonerado, 30=Inafecto, etc.
        if (idTipoAfectoIGV.HasValue)
        {
            return idTipoAfectoIGV.Value switch
            {
                1 => "10",  // Gravado – Operación Onerosa
                2 => "11",  // Gravado – Retiro por premio
                3 => "12",  // Gravado – Retiro por donación
                4 => "13",  // Gravado – Retiro
                5 => "14",  // Gravado – Retiro por publicidad
                6 => "15",  // Gravado – Bonificaciones
                7 => "16",  // Gravado – Retiro por entrega a trabajadores
                8 => "17",  // Gravado – IVAP
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
                _ => flagExonerado ? "20" : "10"
            };
        }
        return flagExonerado ? "20" : "10";
    }

    private static string ImporteEnLetras(decimal importe, string moneda)
    {
        string monedaNombre = moneda == CurrencyUsd ? "DÓLARES AMERICANOS" : "SOLES";
        long entero = (long)Math.Floor(importe);
        int centavos = (int)Math.Round((importe - Math.Floor(importe)) * 100);
        return $"{NumeroEnLetras(entero)} Y {centavos:D2}/100 {monedaNombre}";
    }

    private static string NumeroEnLetras(long numero)
    {
        if (numero == 0) return "CERO";
        if (numero < 0) return "MENOS " + NumeroEnLetras(-numero);

        string[] unidades = ["", "UNO", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE",
            "DIEZ", "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISÉIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE"];
        string[] decenas = ["", "DIEZ", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA"];
        string[] centenas = ["", "CIEN", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS",
            "SEISCIENTOS", "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS"];

        if (numero < 20) return unidades[numero];
        if (numero < 100)
        {
            string resto = numero % 10 != 0 ? " Y " + unidades[numero % 10] : "";
            return decenas[numero / 10] + resto;
        }
        if (numero < 1000)
        {
            if (numero == 100) return "CIEN";
            string resto = numero % 100 != 0 ? " " + NumeroEnLetras(numero % 100) : "";
            return centenas[numero / 100] + resto;
        }
        if (numero < 2000) return "MIL" + (numero % 1000 != 0 ? " " + NumeroEnLetras(numero % 1000) : "");
        if (numero < 1_000_000)
        {
            string miles = NumeroEnLetras(numero / 1000) + " MIL";
            string resto = numero % 1000 != 0 ? " " + NumeroEnLetras(numero % 1000) : "";
            return miles + resto;
        }
        if (numero < 2_000_000) return "UN MILLÓN" + (numero % 1_000_000 != 0 ? " " + NumeroEnLetras(numero % 1_000_000) : "");
        {
            string millones = NumeroEnLetras(numero / 1_000_000) + " MILLONES";
            string resto = numero % 1_000_000 != 0 ? " " + NumeroEnLetras(numero % 1_000_000) : "";
            return millones + resto;
        }
    }
}
