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

    /// <summary>
    /// Catálogo de tipos de transferencia de almacén (tabla TipoTransferenciaAlm).
    /// Tipo 'S' = Salida, Tipo 'I' = Ingreso.
    /// </summary>
    /// <summary>
    /// Subconjunto de políticas de seguridad relevantes para el flujo de ventas.
    /// Valores completos en Security.Politic (BD).
    /// </summary>
    public enum EPolitica : int
    {
        HabilitarVentaSinStock = 7400,  // FlagVentaConStock = !HasPolitic — controla si se genera movimiento de almacén
    }

    public enum ETipoTransferencia : int
    {
        Compra                       = 1011,
        CompraDevolucion             = 1012,
        CompraZofra                  = 1013,
        CompraSalida                 = 1014,
        Importacion                  = 1021,
        TercerosIngreso              = 1031,
        TercerosSalida               = 1032,
        VentaDevolucion              = 1111,
        VentaConfirmar               = 1102,
        Venta                        = 1112,  // Tipo='S', TipoOpera=1, FlagMovimiento=2
        VentaZofra                   = 1113,
        Exportacion                  = 1122,
        VentaDevolucionLibre         = 1151,
        UsoDevolucion                = 1131,
        Uso                          = 1132,
        Muestra                      = 1142,
        MuestraDevolucion            = 1141,
        ProduccionIngreso            = 1211,
        Produccion                   = 1232,
        ProduccionDevolucion         = 1231,
        DiferenciaIngreso            = 1241,
        DiferenciaSalida             = 1242,
        InventarioIngreso            = 1271,
        InventarioSalida             = 1272,
        RecepcionInterna             = 1291,
        TransferenciaInterna         = 1292,
        Recepcion                    = 2121,
        Transferencia                = 2122,
        PrestamoDevolucion           = 2131,
        Prestamo                     = 2132,
        ServicioTercerosIngreso      = 2151,
        ServicioTercerosSalida       = 2152,
        ConsignacionDevolucion       = 2161,
        Recuperacion                 = 2171,
        Deterioro                    = 2172,
        MermaIngreso                 = 2175,
        MermaSalida                  = 2176,
        ExcesoIngreso                = 2177,
        ExcesoSalida                 = 2178,
        TrasladoEnvases              = 2181,
        TrasladoBienes               = 4013,
        Otros                        = 3222,
        Consignacion                 = 3223,
    }

    /// <summary>
    /// Tipo de proceso origen/destino en GuiaRemision (tabla TipoProceso, campo TipoProceso/TipoProceso2).
    /// Valores confirmados desde comentarios en SPs de la BD.
    /// </summary>
    public enum ETipoProceso : byte
    {
        Venta           = 5,   // Confirmado: grr.TipoProceso = 5 --Venta
        OrdenTrabajo    = 113, // Confirmado: @TipoProceso = 113 -- Orden de Trabajo
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
