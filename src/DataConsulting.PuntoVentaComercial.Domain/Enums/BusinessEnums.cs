namespace DataConsulting.PuntoVentaComercial.Domain.Enums
{
    public enum EEstadoVenta : int
    {
        Aprobado = 2,
        Denegado = 3,
        Clasificado = 4,
        Procesado = 5
    }
    public enum EEstadoCompra : int
    {
        Creado = 0,
        Publicado = 1,
        Aprobado = 2,
        Denegado = 3,
        Clasificado = 4,
        Detraccion = 5,
        Procesado = 6
    }

    public enum EEstadoPedido : byte
    {
        Todos = 0,
        Pendiente = 1,
        Atendido = 2,
        AtendidoParcial = 3,
        Anulado = 4
    }

    public enum ETipoVenta : int
    {
        Todos = 0,
        FacturasYBoletasElectronicas = 1,
        NotasElectrónicas = 2,
        FacturasElectronicasExportacion = 3,
        FacturaElectronicaHospedaje = 4,
        FacturaElectronicaAuto = 5,
        FacturaElectronicaMinera = 6,
        GuiaRemisionElectronica = 7,
        GuiaRemisionTransportista = 8
    }

    public enum ETipoAsignacion : short
    {
        Todos = 0,
        Maniobra = 21601,
        Travesia = 21602
    }

    public enum ETipoCliente
    {
        ClienteLocal = 1,
        ClienteNacional = 2,
        ClienteExtranjero = 3
    }

    public enum ECliente
    {
        Varios = 1
    }

    public enum EFlagSexo : byte
    {
        Empresa = 0,
        Masculino = 1,
        Femenino = 2
    }

    // SUNAT Catálogo 07 — Tipo de afectación al IGV
    public enum ETipoAfectacionIGV : int
    {
        GravadoOneroso          = 10,
        GravadoRetiro           = 11,
        GravadoItinerante       = 12,
        GravadoNoDomiciliado    = 13,
        GravadoConcesion        = 14,
        GravadoSinEfectoIGV     = 15,
        GravadoOtros            = 16,
        ExoneradoOneroso        = 20,
        ExoneradoRetiro         = 21,
        InafectoOneroso         = 30,
        InafectoRetiro          = 31,
        InafectoRetiroEmpresa   = 32,
        InafectoTransferencia   = 33,
        InafectoItinerante      = 34,
        InafectoConcesion       = 35,
        InafectoOtros           = 36,
        Exportacion             = 40
    }
}
