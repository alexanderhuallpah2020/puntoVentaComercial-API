namespace DataConsulting.PuntoVentaComercial.Domain.Enums
{
    public enum EDocumento : int
    {
        Todos = 0,
        GuiaRemision = 6,
        GuiaRemisionTransportista = 7,
        OrdenCompra = 27,
        Factura = 9,
        Boleta = 10,
        NotaCredito = 15,
        NotaDebito = 16,
        TicketBoleta = 17,
        TicketFactura = 18,
        NotaCréditoNoDomiciliado = 166,
        NotaDébitoNoDomiciliado = 167,
        NotaCréditoEspecial = 168,
        NotaDébitoEspecial = 169,
        FacturaElectronica = 184,
        BoletaElectronica = 191,
        NotaCreditoElectronica = 186,
        NotaDebitoElectronica = 187,
        RetencionElectronica = 130,
        TicketMaquinaRegistradora = 12
    }

    public enum EDocumentoIdentidad : byte
    {
        Todos = 0,
        DNI = 1,
        RUC = 2,
        CIDENTIDAD = 3,
        PASAPORTE = 4,
        CARNETDEEXTRANJERIA = 6,
        CEP = 7,
        RIF = 8,
        CNP = 9,
        OTRO = 10,
        CNPJ = 11,
        RUT = 12,
        CIF = 13,
        NIF = 14,
        COMPROBANTEDENODOMICILIADO = 214
    }

    public enum ETipoEstadoSunat : byte
    {
        Todos = 0,
        NoEnviado = 1,
        Aceptado = 2,
        Rechazado = 3,
        Enviado = 4,
        Emitido = 5,
        Restante = 6,
        Anulado = 7,
        DadoBaja = 8
    }

    public enum EMotivoTraslado : int
    {
        Venta = 1,
        Compra = 2,
        TrasladoEstablecimiento = 3,
        Importación = 4,
        Exportación = 5,
        Otros = 6,
        VentaSujetaComprador = 7,
        TrasladoEmisorItinerante = 8,
        TrasladoZonaPrimaria = 9,
        Devolucion = 10,
        TrasladoBienesTransformacion = 11,
        VentaEntregaTerceros = 12,
        Consignacion = 13,
        RecojoBienesTransformados = 14
    }
    public enum EMotivoContingencia : int
    {
        Todos = 0,
        ConexionInternet = 1,
        FallaFluidoElectrico = 2,
        DesastresNaturales = 3,
        Robo = 4,
        FallaSistemaFacturacion = 5,
        VentaItinerante = 6,
        Otros = 7
    }
    public enum ETipoVentaSunat : int
    {
        VentaInterna = 1,
        VentaAnticipos = 2,
        VentaItinerante = 3,
        ExportacionBienes = 4,
        ExportacionServiciosPais = 5,
        ExportacionHospedaje = 6,
        VentaDetraccion = 7
    }

    public enum ETipoPrecio : int
    {
        Todos = 0,
        Afecto = 1,
        Inafecto = 2,
        Exonerado = 3,
        Regalo = 4,
        Donacion = 5,
        ICPBER = 6
    }

    // Comprobantes de pago usados en el POS (subconjunto de EDocumento)
    public enum ETipoComprobante : short
    {
        Factura      = 9,
        Boleta       = 10,
        Ticket       = 12,
        NotaCredito  = 15,
        NotaDebito   = 16
    }

}
