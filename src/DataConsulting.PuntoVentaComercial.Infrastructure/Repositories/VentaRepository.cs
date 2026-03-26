using DataConsulting.PuntoVentaComercial.Domain.Ventas;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories;

internal sealed class VentaRepository(ApplicationDbContext dbContext)
    : Repository<Venta>(dbContext), IVentaRepository
{
    public new async Task<Venta?> GetByIdAsync(int idVenta, CancellationToken ct) =>
        await DbContext.Ventas
            .Include(x => x.Detalles)
            .Include(x => x.Pagos)
            .FirstOrDefaultAsync(x => x.Id == idVenta, ct);

    public async Task<(IList<VentaSearchResultDto> Items, int Total)> SearchAsync(
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        string? nombreCliente,
        string? numSerieA,
        int? numDocumento,
        short? idTipoDocumento,
        string? estado,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query =
            from v in DbContext.Ventas.AsNoTracking()
            join c in DbContext.Clientes.AsNoTracking() on v.IdCliente equals c.Id
            select new { Venta = v, ClienteNombre = c.Nombre };

        if (fechaDesde.HasValue)
            query = query.Where(x => x.Venta.FechaEmision >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(x => x.Venta.FechaEmision < fechaHasta.Value.Date.AddDays(1));

        if (!string.IsNullOrWhiteSpace(nombreCliente))
            query = query.Where(x => x.ClienteNombre.Contains(nombreCliente));

        if (!string.IsNullOrWhiteSpace(numSerieA))
            query = query.Where(x => x.Venta.NumSerieA == numSerieA);

        if (numDocumento.HasValue)
        {
            string numDocumentoA = numDocumento.Value.ToString("D6");
            query = query.Where(x => x.Venta.NumeroDocumentoA == numDocumentoA);
        }

        if (idTipoDocumento.HasValue)
            query = query.Where(x => x.Venta.IdTipoDocumento == idTipoDocumento.Value);

        if (!string.IsNullOrWhiteSpace(estado))
            query = query.Where(x => x.Venta.Estado == estado);

        int total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(x => x.Venta.FechaEmision)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new VentaSearchResultDto(
                x.Venta.Id,
                x.Venta.IdTipoDocumento,
                x.Venta.NumSerieA,
                x.Venta.NumeroDocumentoA,
                x.ClienteNombre,
                x.Venta.Vendedor,
                x.Venta.FechaEmision,
                x.Venta.Estado,
                x.Venta.ImporteTotal))
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<int> GetNroCorrelativoVentaAsync(
        DateTime fechaEmision, short idSubdiario, CancellationToken ct)
    {
        // SP devuelve MAX(NroCorrelativo) del mes/año/subdiario; el handler suma +1
        var result = await DbContext.Database
            .SqlQuery<int>($"EXEC dbo.GetNroCorrelativoVenta {fechaEmision}, {idSubdiario}")
            .ToListAsync(ct);
        return result.FirstOrDefault();
    }

    public async Task<(string? CodigoSunatRespuesta, string? EstadoSunat, string? CodigoSunatDocumento)>
        BuscarCodigoSunatVentaAsync(int idVenta, CancellationToken ct)
    {
        var row = await (
            from v in DbContext.Ventas.AsNoTracking()
            join d in DbContext.Documentos.AsNoTracking() on v.IdTipoDocumento equals d.IdTipoDocumento
            where v.Id == idVenta
            select new
            {
                CodigoSunatRespuesta = v.CodigoSunat,
                v.EstadoSunat,
                CodigoSunatDocumento = d.CodigoSunat
            }
        ).FirstOrDefaultAsync(ct);

        return (row?.CodigoSunatRespuesta, row?.EstadoSunat, row?.CodigoSunatDocumento);
    }

    public async Task InsVentaXmlLogAsync(
        int idVenta, string nombreXml, string nombreZip, int resultado, CancellationToken ct)
    {
        await DbContext.Database.ExecuteSqlRawAsync(
            "EXEC dbo.InsVentaXmlLog @IdVenta, @Fecha, @NombreXml, @NombreZip, @IdUsuario, @Resultado",
            new Microsoft.Data.SqlClient.SqlParameter("@IdVenta", idVenta),
            new Microsoft.Data.SqlClient.SqlParameter("@Fecha", DateTime.Now),
            new Microsoft.Data.SqlClient.SqlParameter("@NombreXml", nombreXml),
            new Microsoft.Data.SqlClient.SqlParameter("@NombreZip", nombreZip),
            new Microsoft.Data.SqlClient.SqlParameter("@IdUsuario", 1),
            new Microsoft.Data.SqlClient.SqlParameter("@Resultado", resultado));
    }

    public async Task UpdVentaArchivoXmlAsync(
        int idVenta, byte[] xmlBytes, string nombreXml,
        byte[]? cdrBytes, string? nombreCdr, string usuario, CancellationToken ct)
    {
        await DbContext.Database.ExecuteSqlRawAsync(
            "EXEC dbo.UpdVentaArchivoXML @IdVenta, @ArchivoXML, @NombreArchivoXML, @RespuestaXML, @NombreRespuestaXML, @Usuario",
            new Microsoft.Data.SqlClient.SqlParameter("@IdVenta", idVenta),
            new Microsoft.Data.SqlClient.SqlParameter("@ArchivoXML", System.Data.SqlDbType.Image) { Value = xmlBytes },
            new Microsoft.Data.SqlClient.SqlParameter("@NombreArchivoXML", nombreXml),
            new Microsoft.Data.SqlClient.SqlParameter("@RespuestaXML", System.Data.SqlDbType.Image) { Value = (object?)cdrBytes ?? System.DBNull.Value },
            new Microsoft.Data.SqlClient.SqlParameter("@NombreRespuestaXML",
                (object?)nombreCdr ?? System.DBNull.Value),
            new Microsoft.Data.SqlClient.SqlParameter("@Usuario", usuario));
    }

    public async Task UpdEstadoFacturaElectronicaAsync(
        int idVenta, string codigoSunat, string estadoSunat, string estado, CancellationToken ct)
    {
        await DbContext.Database.ExecuteSqlRawAsync(
            "EXEC dbo.UpdEstadoFacturaElectronica @CodigoSunat, @EstadoSunat, @Estado, @IdVenta",
            new SqlParameter("@CodigoSunat", codigoSunat),
            new SqlParameter("@EstadoSunat", estadoSunat),
            new SqlParameter("@Estado", estado),
            new SqlParameter("@IdVenta", idVenta));
    }

    public async Task<int?> GetNextNumeroDocumentoAsync(
        short idSucursal, short idTipoDocumento, string numSerieA, CancellationToken ct)
    {
        short numSerie = 0;
        var result = await DbContext.Database
            .SqlQuery<int?>($"EXEC dbo.GetNuevoCorrelativoDocumento {idSucursal}, {idTipoDocumento}, {numSerie}, {numSerieA}")
            .ToListAsync(ct);
        return result.FirstOrDefault();
    }

    // ── Anulación ────────────────────────────────────────────────────────────

    public async Task<VentaAnulacionGuardsDto?> GetAnulacionGuardsAsync(int idVenta, CancellationToken ct)
    {
        // FlagGuiaRemision y FlagNotaCD no son columnas de dbo.Venta;
        // son calculados por el SP a partir de tablas relacionadas.
        var spRows = await DbContext.Database
            .SqlQuery<VentaGuardsRow>($"EXEC dbo.GetVentaByIdVenta {idVenta}")
            .ToListAsync(ct);

        var row = spRows.FirstOrDefault();
        if (row is null) return null;

        // EstadoContable no está en el SP — se obtiene directamente de la tabla.
        // DEFAULT = 1 significa "no contabilizado"; si el módulo contable lo procesa,
        // cambia a otro valor. Se bloquea la anulación si EstadoContable != 1 (y != null).
        var ecRows = await DbContext.Database
            .SqlQuery<byte?>($"SELECT EstadoContable FROM dbo.Venta WHERE IdVenta = {idVenta}")
            .ToListAsync(ct);
        byte estadoContable = ecRows.FirstOrDefault() ?? 1;

        return new VentaAnulacionGuardsDto(
            TieneGuiaRemision: row.FlagGuiaRemision == 1,
            TieneNotaCD:       row.FlagNotaCD == 1,
            TieneValorCambio:  row.FlagValorCambio == 1,
            EstaContabilizada: estadoContable != 1);
    }

    public async Task<bool> TieneMovimientoIngresoAsync(int idVenta, CancellationToken ct)
    {
        // TipoProceso=5 (Venta), TipoOperacion=0 (Ingreso)
        const byte tipoProceso    = 5;
        const int  tipoOperacion  = 0;
        var rows = await DbContext.Database
            .SqlQuery<GuiaRefRow>(
                $"EXEC dbo.GetGuiaRemisionByIdOperacion {idVenta}, {tipoProceso}, {tipoOperacion}")
            .ToListAsync(ct);
        return rows.Count > 0;
    }

    public async Task<bool> PeriodoAbiertoPorFechaAsync(short idSucursal, DateTime fechaEmision, CancellationToken ct)
    {
        // IdTipoPeriodo=8701 (mensual), IdModulo=8602 (Ventas), IdAlmacen=0
        // BusCierrePeriodo tiene columnas duplicadas → valores inline (sin named params).
        int cierreId = await LeerPrimerIntSpAsync(
            $"EXEC dbo.BusCierrePeriodo {(int)idSucursal}, 8701, 8602, 0", ct);

        if (cierreId == 0) return true; // Sin cierre → período abierto

        // GetCierrePeriodo tiene cp.IdSucursal duplicado → consulta directa a la tabla.
        var estados = await DbContext.Database
            .SqlQuery<byte>($"SELECT Estado FROM dbo.CierrePeriodo WHERE IdCierrePeriodo = {cierreId}")
            .ToListAsync(ct);

        // Estado 2 = Cerrado
        byte estado = estados.FirstOrDefault();
        return estado < 2;
    }

    /// <summary>
    /// Ejecuta SQL con valores inline y retorna el primer entero de la primera fila (0 si vacío/NULL).
    /// Útil cuando el SP devuelve columnas duplicadas que rompen SqlQuery&lt;T&gt;.
    /// </summary>
    private async Task<int> LeerPrimerIntSpAsync(string sql, CancellationToken ct)
    {
        var conn = (Microsoft.Data.SqlClient.SqlConnection)DbContext.Database.GetDbConnection();
        bool wasOpen = conn.State == System.Data.ConnectionState.Open;
        if (!wasOpen) await conn.OpenAsync(ct);
        try
        {
            await using var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync(
                System.Data.CommandBehavior.SingleRow, ct);
            if (await reader.ReadAsync(ct) && !await reader.IsDBNullAsync(0, ct))
                return reader.GetInt32(0);
            return 0;
        }
        finally
        {
            if (!wasOpen) await conn.CloseAsync();
        }
    }

    public async Task AnularVentaCompletaAsync(int idVenta, short idTipoVenta, string usuario, CancellationToken ct)
    {
        // ── Paso 1: VENTA DIRECTA → anular la cobranza (OperacionPago) ───────
        if (idTipoVenta == 3)
        {
            // 1a. Buscar referencia a la cobranza (TipoOperacion=2=CtasXCobrar)
            const byte tipoOpCobranza = 2;
            var opRefs = await DbContext.Database
                .SqlQuery<OperacionPagoRefRow>(
                    $"EXEC dbo.GetOperacionPagoByDocumento {tipoOpCobranza}, {idVenta}, {1}, {1}")
                .ToListAsync(ct);

            var opRef = opRefs.FirstOrDefault();
            if (opRef is not null)
            {
                // 1b. Cargar cabecera de la cobranza (IdEmpresa viene del mismo SP ref)
                var opRows = await DbContext.Database
                    .SqlQuery<OperacionPagoRow>(
                        $"EXEC dbo.GetOperacionPago {opRef.IdEmpresa}, {opRef.TipoOperacion}, {opRef.NroOperacion}")
                    .ToListAsync(ct);
                var op = opRows.FirstOrDefault();

                // 1c. Cargar detalle de la cobranza (sin SP específico — consulta directa)
                var detalles = await DbContext.Database
                    .SqlQuery<OperacionPagoDetalleRow>(
                        $"SELECT TipoOperacion, NroOperacion, Secuencia, IdFormaPago, IdTipoMoneda, Importe, IdDocumentoRef, SecuenciaRef, Estado, NumReferencia, SecuenciaEntidadRef FROM dbo.OperacionPagoDetalle WHERE TipoOperacion = {opRef.TipoOperacion} AND NroOperacion = {opRef.NroOperacion}")
                    .ToListAsync(ct);

                // 1d. Cargar provisión activa (CuentaPendiente Estado=1)
                var provision = await GetCuentaPendienteActivaAsync(tipoOpCobranza, idVenta, ct);

                if (op is not null)
                {
                    // 1e. Anular cabecera OperacionPago (Estado=0)
                    await DbContext.Database.ExecuteSqlRawAsync(
                        "EXEC dbo.UpdOperacionPago @IdEmpresa,@TipoOperacion,@NroOperacion,@FechaEmision,@IdEntidad,@IdTipoMoneda," +
                        "@ImporteTotal,@Retenciones,@Descuentos,@ImportePago,@IdSucursal,@IdTrabajador,@IdEstacion,@Estado," +
                        "@TipoCambio,@Observaciones,@UpdateToken,@Usuario,@IdTurnoAsistencia,@IdDocumentoRef," +
                        "@Detracciones,@Percepciones,@IdTipoDocumentoRef,@IdConceptoPago,@NumSerieDocumentoRef," +
                        "@IdLiquidacion,@IdCajaChica,@FlagRendido,@IdRendicionCajaChica,@IdConceptoCtaCte",
                        BuildOperacionPagoParams(op, estado: 0, usuario));

                    // 1f. Anular cada detalle (Estado=0)
                    foreach (var d in detalles)
                        await DbContext.Database.ExecuteSqlRawAsync(
                            "EXEC dbo.UpdOperacionPagoDetalle @IdEmpresa,@TipoOperacion,@NroOperacion,@Secuencia," +
                            "@IdFormaPago,@IdTipoMoneda,@Importe,@IdDocumentoRef,@SecuenciaRef,@Estado,@NumReferencia,@SecuenciaEntidadRef",
                            BuildOperacionPagoDetalleParams(opRef.IdEmpresa, d, estado: 0));

                    // 1g. Anular amortización de la provisión (CuentaAmortizacion Estado=0)
                    if (provision is not null)
                        await DbContext.Database.ExecuteSqlRawAsync(
                            "EXEC dbo.UpdCuentaAmortizacion @IdEmpresa,@TipoOperacion,@NroOperacion," +
                            "@TipoOperacionRef,@IdOperacion,@Secuencia,@Importe,@Retencion,@Descuento," +
                            "@Estado,@Detraccion,@Percepcion",
                            BuildCuentaAmortizacionParams(opRef.IdEmpresa, op, provision, estado: 0));
                }

                // 1h. Restaurar CuentaPendiente a Estado=1 (revertir cobro)
                if (provision is not null)
                    await DbContext.Database.ExecuteSqlRawAsync(
                        "EXEC dbo.UpdCuentaPendiente @IdEmpresa,@TipoOperacion,@IdOperacion,@Secuencia,@IdTipoMoneda," +
                        "@Importe,@Saldo,@Descuentos,@Retenciones,@SaldoRetencion,@FechaPago,@Estado,@IdEntidad," +
                        "@UpdateToken,@Usuario,@Detracciones,@SaldoDetraccion,@FlagTipo,@IdOperacionNew,@SecuenciaNew," +
                        "@MontoInteres,@Glosa,@FlagFacturado,@IdTipoDocumento,@Percepciones,@SaldoPercepcion," +
                        "@IdOperacionRef,@FlagInicial,@FlagLiquidado",
                        BuildCuentaPendienteParams(provision, estado: 1, usuario));
            }
        }

        // ── Paso 2: SP principal de anulación ────────────────────────────────
        await DbContext.Database.ExecuteSqlRawAsync(
            "EXEC dbo.ExtornarVenta @IdVenta, @Usuario",
            new SqlParameter("@IdVenta", idVenta),
            new SqlParameter("@Usuario", usuario));

        // ── Paso 3: Anular la provisión (re-lee para obtener UpdateToken fresco)
        var provisionFinal = await GetCuentaPendienteActivaAsync(2, idVenta, ct);
        if (provisionFinal is not null)
            await DbContext.Database.ExecuteSqlRawAsync(
                "EXEC dbo.UpdCuentaPendiente @IdEmpresa,@TipoOperacion,@IdOperacion,@Secuencia,@IdTipoMoneda," +
                "@Importe,@Saldo,@Descuentos,@Retenciones,@SaldoRetencion,@FechaPago,@Estado,@IdEntidad," +
                "@UpdateToken,@Usuario,@Detracciones,@SaldoDetraccion,@FlagTipo,@IdOperacionNew,@SecuenciaNew," +
                "@MontoInteres,@Glosa,@FlagFacturado,@IdTipoDocumento,@Percepciones,@SaldoPercepcion," +
                "@IdOperacionRef,@FlagInicial,@FlagLiquidado",
                BuildCuentaPendienteParams(provisionFinal, estado: 0, usuario));
    }

    // ── Helpers privados: lectura ─────────────────────────────────────────────

    private async Task<CuentaPendienteRow?> GetCuentaPendienteActivaAsync(
        byte tipoOperacion, int idOperacion, CancellationToken ct)
    {
        // GetCuentaPendiente tiene FechaPago duplicado → consulta directa a la tabla.
        var rows = await DbContext.Database
            .SqlQuery<CuentaPendienteRow>($"""
                SELECT TOP 1 IdEmpresa, TipoOperacion, IdOperacion, Secuencia, IdTipoMoneda,
                    Importe, Saldo, Descuentos, Retenciones, SaldoRetencion, FechaPago, Estado,
                    IdEntidad, UpdateToken, Detracciones, SaldoDetraccion, FlagTipo,
                    MontoInteres, Glosa, FlagFacturado, IdTipoDocumento, Percepciones,
                    SaldoPercepcion, IdOperacionRef, FlagInicial, FlagLiquidado
                FROM dbo.CuentaPendiente
                WHERE TipoOperacion = {tipoOperacion} AND IdOperacion = {idOperacion} AND Estado = 1
                """)
            .ToListAsync(ct);
        return rows.FirstOrDefault();
    }

    // ── Helpers privados: construcción de parámetros ──────────────────────────

    private static SqlParameter[] BuildOperacionPagoParams(
        OperacionPagoRow op, int estado, string usuario) =>
    [
        new("@IdEmpresa",            op.IdEmpresa),
        new("@TipoOperacion",        op.TipoOperacion),
        new("@NroOperacion",         op.NroOperacion),
        new("@FechaEmision",         op.FechaEmision),
        new("@IdEntidad",            op.IdEntidad),
        new("@IdTipoMoneda",         op.IdTipoMoneda),
        new("@ImporteTotal",         op.ImporteTotal),
        new("@Retenciones",          op.Retenciones),
        new("@Descuentos",           op.Descuentos),
        new("@ImportePago",          op.ImportePago),
        new("@IdSucursal",           op.IdSucursal),
        new("@IdTrabajador",         op.IdTrabajador),
        new("@IdEstacion",           op.IdEstacion),
        new("@Estado",               estado),
        new("@TipoCambio",           op.TipoCambio),
        new("@Observaciones",        (object?)op.Observaciones ?? DBNull.Value),
        new("@UpdateToken",          op.UpdateToken),
        new("@Usuario",              usuario),
        new("@IdTurnoAsistencia",    (object?)op.IdTurnoAsistencia    ?? DBNull.Value),
        new("@IdDocumentoRef",       (object?)op.IdDocumentoRef       ?? DBNull.Value),
        new("@Detracciones",         op.Detracciones),
        new("@Percepciones",         op.Percepciones),
        new("@IdTipoDocumentoRef",   (object?)op.IdTipoDocumentoRef   ?? DBNull.Value),
        new("@IdConceptoPago",       (object?)op.IdConceptoPago       ?? DBNull.Value),
        new("@NumSerieDocumentoRef", (object?)op.NumSerieDocumentoRef ?? DBNull.Value),
        new("@IdLiquidacion",        (object?)op.IdLiquidacion        ?? DBNull.Value),
        new("@IdCajaChica",          (object?)op.IdCajaChica          ?? DBNull.Value),
        new("@FlagRendido",          (object?)op.FlagRendido          ?? DBNull.Value),
        new("@IdRendicionCajaChica", (object?)op.IdRendicionCajaChica ?? DBNull.Value),
        new("@IdConceptoCtaCte",     (object?)op.IdConceptoCtaCte     ?? DBNull.Value),
    ];

    private static SqlParameter[] BuildOperacionPagoDetalleParams(
        int idEmpresa, OperacionPagoDetalleRow d, int estado) =>
    [
        new("@IdEmpresa",          idEmpresa),
        new("@TipoOperacion",      d.TipoOperacion),
        new("@NroOperacion",       d.NroOperacion),
        new("@Secuencia",          d.Secuencia),
        new("@IdFormaPago",        d.IdFormaPago),
        new("@IdTipoMoneda",       d.IdTipoMoneda),
        new("@Importe",            d.Importe),
        new("@IdDocumentoRef",     (object?)d.IdDocumentoRef  ?? DBNull.Value),
        new("@SecuenciaRef",       (object?)d.SecuenciaRef    ?? DBNull.Value),
        new("@Estado",             estado),
        new("@NumReferencia",      (object?)d.NumReferencia   ?? DBNull.Value),
        new("@SecuenciaEntidadRef",(object?)d.SecuenciaEntidadRef ?? DBNull.Value),
    ];

    private static SqlParameter[] BuildCuentaAmortizacionParams(
        int idEmpresa, OperacionPagoRow op, CuentaPendienteRow cp, int estado) =>
    [
        new("@IdEmpresa",       idEmpresa),
        new("@TipoOperacion",   op.TipoOperacion),
        new("@NroOperacion",    op.NroOperacion),
        new("@TipoOperacionRef",op.TipoOperacion),   // misma operación
        new("@IdOperacion",     cp.IdOperacion),
        new("@Secuencia",       cp.Secuencia),
        new("@Importe",         cp.Importe),
        new("@Retencion",       cp.Retenciones),
        new("@Descuento",       cp.Descuentos),
        new("@Estado",          estado),
        new("@Detraccion",      cp.Detracciones),
        new("@Percepcion",      cp.Percepciones),
    ];

    private static SqlParameter[] BuildCuentaPendienteParams(
        CuentaPendienteRow cp, int estado, string usuario) =>
    [
        new("@IdEmpresa",       cp.IdEmpresa),
        new("@TipoOperacion",   cp.TipoOperacion),
        new("@IdOperacion",     cp.IdOperacion),
        new("@Secuencia",       cp.Secuencia),
        new("@IdTipoMoneda",    cp.IdTipoMoneda),
        new("@Importe",         cp.Importe),
        new("@Saldo",           cp.Saldo),
        new("@Descuentos",      cp.Descuentos),
        new("@Retenciones",     cp.Retenciones),
        new("@SaldoRetencion",  cp.SaldoRetencion),
        new("@FechaPago",       cp.FechaPago),
        new("@Estado",          estado),
        new("@IdEntidad",       cp.IdEntidad),
        new("@UpdateToken",     cp.UpdateToken),
        new("@Usuario",         usuario),
        new("@Detracciones",    cp.Detracciones),
        new("@SaldoDetraccion", cp.SaldoDetraccion),
        new("@FlagTipo",        cp.FlagTipo),
        new("@IdOperacionNew",  cp.IdOperacion),   // OUTPUT param: mantener la misma PK
        new("@SecuenciaNew",    (int)cp.Secuencia), // OUTPUT param: mantener la misma PK
        new("@MontoInteres",    cp.MontoInteres),
        new("@Glosa",           (object?)cp.Glosa          ?? DBNull.Value),
        new("@FlagFacturado",   cp.FlagFacturado),
        new("@IdTipoDocumento", cp.IdTipoDocumento),
        new("@Percepciones",    cp.Percepciones),
        new("@SaldoPercepcion", cp.SaldoPercepcion),
        new("@IdOperacionRef",  (object?)cp.IdOperacionRef ?? DBNull.Value),
        new("@FlagInicial",     cp.FlagInicial),
        new("@FlagLiquidado",   cp.FlagLiquidado),
    ];

    // ── Clases de proyección privadas (solo Infrastructure) ──────────────────

    // Solo declara las columnas que necesitamos; EF Core ignora las demás del SP.
    // EstadoContable NO está en GetVentaByIdVenta — se consulta por separado.
    // FlagGuiaRemision/FlagNotaCD son CASE…THEN 1 ELSE 0 → INT.
    // FlagValorCambio es columna TINYINT → byte.
    private sealed class VentaGuardsRow
    {
        public int  FlagGuiaRemision { get; set; }
        public int  FlagNotaCD       { get; set; }
        public byte FlagValorCambio  { get; set; }
    }

    private sealed class GuiaRefRow
    {
        public int IdGuiaRemision { get; set; }
    }

    // GetOperacionPagoByDocumento retorna: IdEmpresa, TipoOperacion, NroOperacion, IdCajaChica
    private sealed class OperacionPagoRefRow
    {
        public int  IdEmpresa     { get; set; }
        public int  TipoOperacion { get; set; }
        public int  NroOperacion  { get; set; }
        public int? IdCajaChica   { get; set; }
    }

    private sealed class OperacionPagoRow
    {
        public int      IdEmpresa            { get; set; }
        public int      TipoOperacion        { get; set; }
        public int      NroOperacion         { get; set; }
        public DateTime FechaEmision         { get; set; }
        public int      IdEntidad            { get; set; }
        public int      IdTipoMoneda         { get; set; }
        public decimal  ImporteTotal         { get; set; }
        public decimal  Retenciones          { get; set; }
        public decimal  Descuentos           { get; set; }
        public decimal  ImportePago          { get; set; }
        public int      IdSucursal           { get; set; }
        public int      IdTrabajador         { get; set; }
        public int      IdEstacion           { get; set; }
        public int      Estado               { get; set; }
        public decimal  TipoCambio           { get; set; }
        public string?  Observaciones        { get; set; }
        public int      UpdateToken          { get; set; }
        public int?     IdTurnoAsistencia    { get; set; }
        public int?     IdDocumentoRef       { get; set; }
        public decimal  Detracciones         { get; set; }
        public decimal  Percepciones         { get; set; }
        public int?     IdTipoDocumentoRef   { get; set; }
        public int?     IdConceptoPago       { get; set; }
        public string?  NumSerieDocumentoRef { get; set; }
        public int?     IdLiquidacion        { get; set; }
        public int?     IdCajaChica          { get; set; }
        public int?     FlagRendido          { get; set; }
        public int?     IdRendicionCajaChica { get; set; }
        public int?     IdConceptoCtaCte     { get; set; }
    }

    private sealed class OperacionPagoDetalleRow
    {
        public int      TipoOperacion      { get; set; }
        public int      NroOperacion       { get; set; }
        public int      Secuencia          { get; set; }
        public int      IdFormaPago        { get; set; }
        public int      IdTipoMoneda       { get; set; }
        public decimal  Importe            { get; set; }
        public int?     IdDocumentoRef     { get; set; }
        public int?     SecuenciaRef       { get; set; }
        public int      Estado             { get; set; }
        public string?  NumReferencia      { get; set; }
        public int?     SecuenciaEntidadRef{ get; set; }
    }

    // Tipos exactos según DDL de dbo.CuentaPendiente.
    // IdOperacionNew/SecuenciaNew son OUTPUT params de UpdCuentaPendiente, NO columnas de la tabla.
    private sealed class CuentaPendienteRow
    {
        public short    IdEmpresa      { get; set; }  // SMALLINT
        public byte     TipoOperacion  { get; set; }  // TINYINT
        public int      IdOperacion    { get; set; }  // INT
        public short    Secuencia      { get; set; }  // SMALLINT
        public short    IdTipoMoneda   { get; set; }  // SMALLINT
        public decimal  Importe        { get; set; }  // MONEY
        public decimal  Saldo          { get; set; }  // MONEY
        public decimal  Descuentos     { get; set; }  // MONEY
        public decimal  Retenciones    { get; set; }  // MONEY
        public decimal  SaldoRetencion { get; set; }  // MONEY
        public DateTime FechaPago      { get; set; }  // SMALLDATETIME
        public byte     Estado         { get; set; }  // TINYINT
        public int      IdEntidad      { get; set; }  // INT
        public byte     UpdateToken    { get; set; }  // TINYINT
        public decimal  Detracciones   { get; set; }  // MONEY
        public decimal  SaldoDetraccion{ get; set; }  // MONEY
        public byte     FlagTipo       { get; set; }  // TINYINT
        public decimal  MontoInteres   { get; set; }  // MONEY
        public string   Glosa          { get; set; } = string.Empty; // VARCHAR NOT NULL
        public byte     FlagFacturado  { get; set; }  // TINYINT
        public short    IdTipoDocumento{ get; set; }  // SMALLINT
        public decimal  Percepciones   { get; set; }  // MONEY
        public decimal  SaldoPercepcion{ get; set; }  // MONEY
        public int?     IdOperacionRef { get; set; }  // INT NULL
        public byte     FlagInicial    { get; set; }  // TINYINT
        public byte     FlagLiquidado  { get; set; }  // TINYINT
    }
}
