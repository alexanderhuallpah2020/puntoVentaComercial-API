using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Services;

internal sealed class XmlSignerService : IXmlSignerService
{
    public void Sign(XmlDocument xmlDoc, string certPath, string certPassword)
    {
        var cert = X509CertificateLoader.LoadPkcs12FromFile(certPath, certPassword,
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

        xmlDoc.PreserveWhitespace = true;

        var signedXml = new SignedXml(xmlDoc);
        signedXml.SigningKey = cert.GetRSAPrivateKey()
            ?? throw new InvalidOperationException("Certificate has no RSA private key.");

        var reference = new Reference { Uri = "" };
        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        signedXml.AddReference(reference);

        var x509Data = new KeyInfoX509Data();
        x509Data.AddCertificate(cert);
        var keyInfo = new KeyInfo();
        keyInfo.AddClause(x509Data);
        signedXml.KeyInfo = keyInfo;

        signedXml.ComputeSignature();

        XmlElement signature = signedXml.GetXml();

        // SUNAT requires ds: prefix on all signature nodes
        SetPrefix("ds", signature);

        // Add Id="SignatureSP" attribute to the root Signature element
        var idAttr = xmlDoc.CreateAttribute("Id");
        idAttr.Value = "SignatureSP";
        signature.Attributes.Append(idAttr);

        // Recompute after prefix changes
        signedXml.ComputeSignature();
        signature = signedXml.GetXml();
        SetPrefix("ds", signature);
        idAttr = xmlDoc.CreateAttribute("Id");
        idAttr.Value = "SignatureSP";
        signature.Attributes.Append(idAttr);

        // Inject into ext:ExtensionContent[0]
        var extensionContent = xmlDoc.GetElementsByTagName("ext:ExtensionContent").Item(0)
            ?? throw new InvalidOperationException("ext:ExtensionContent element not found in XML template.");
        extensionContent.AppendChild(xmlDoc.ImportNode(signature, true));
    }

    private static void SetPrefix(string prefix, XmlNode node)
    {
        if (node.NamespaceURI == "http://www.w3.org/2000/09/xmldsig#")
            node.Prefix = prefix;

        foreach (XmlNode child in node.ChildNodes)
            SetPrefix(prefix, child);
    }
}
