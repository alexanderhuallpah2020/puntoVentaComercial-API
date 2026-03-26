namespace DataConsulting.PuntoVentaComercial.Domain.Ventas;

public static class CodigosSunat
{
    // Catálogo 01 — Tipo de documento
    public const string Factura     = "01";
    public const string Boleta      = "03";
    public const string NotaCredito = "07";
    public const string NotaDebito  = "08";

    // Respuesta SUNAT almacenada en dbo.Venta.CodigoSunat
    // null  → nunca enviado
    // "0"   → aceptado por SUNAT
    // otro  → código de error devuelto por SUNAT
    public const string Aceptado = "0";
}
