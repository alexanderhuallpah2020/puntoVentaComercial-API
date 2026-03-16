namespace DataConsulting.PuntoVentaComercial.Domain.Enums
{
    /// <summary>
    /// Catálogo SUNAT 07 — Tipo de afectación al IGV.
    /// </summary>
    public enum ETipoAfectacionIgv : int
    {
        Gravado = 10,
        GravadoRetiro = 11,
        GravadoMerma = 12,
        GravadoConsumo = 13,
        GravadoBonificacion = 14,
        Gratuito = 15,
        Exonerado = 20,
        ExoneradoTransferencia = 21,
        Inafecto = 30,
        InafectoRetiro = 31,
        InafectoRetiroBonificacion = 32,
        InafectoTransferencia = 33,
        Exportacion = 40
    }

    /// <summary>
    /// Sistema de cálculo del ISC (Impuesto Selectivo al Consumo).
    /// </summary>
    public enum ETipoIsc : int
    {
        AlValor = 1,
        Especifico = 2,
        AlPrecioDeVenta = 3
    }

    /// <summary>
    /// Tipo de descuento aplicado en una línea o a nivel global.
    /// </summary>
    public enum ETipoDescuento : int
    {
        PorcentajeItem = 1,
        MontoItem = 2,
        PorcentajeGlobal = 3,
        MontoGlobal = 4
    }

    /// <summary>
    /// Forma de pago que determina la condición comercial (contado / crédito).
    /// </summary>
    public enum EFormaPago : int
    {
        Contado = 1,
        Credito = 2
    }

    /// <summary>
    /// Tipo de cliente para reglas de negocio internas.
    /// </summary>
    public enum ETipoCliente : int
    {
        Regular = 1,
        Mayorista = 2,
        VIP = 3
    }
}
