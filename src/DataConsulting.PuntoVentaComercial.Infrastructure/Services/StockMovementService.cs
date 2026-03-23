using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;
using DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.CreateVenta;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Services;

internal sealed class StockMovementService(ApplicationDbContext dbContext) : IStockMovementService
{

    public async Task DescuentoStockVentaAsync(
        short idEmpresa,
        short idSucursal,
        int idVenta,
        int idCliente,
        short idVendedor,
        short idTipoMoneda,
        decimal importeTotal,
        IList<CreateVentaDetalleDto> detalles,
        CancellationToken cancellationToken)
    {
        var conn = dbContext.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await conn.OpenAsync(cancellationToken);

        // Enlista los comandos ADO.NET en la transacción EF activa (misma conexión)
        var tx = dbContext.Database.CurrentTransaction?.GetDbTransaction();

        // 1. PuntoPartida = Sucursal.Direccion
        string puntoPartida = await GetSucursalDireccionAsync(conn, tx, idSucursal, cancellationToken);

        // 2. NumSerie para Parte de Salida (IdTipoDocumento=30)
        short numSerie = await GetNumSerieDocumentoAsync(conn, tx, idEmpresa, idSucursal, cancellationToken);

        // 3. Agrupar detalles por IdLocacion
        var grupos = detalles
            .Select((d, i) => (Detalle: d, Correlativo: (short)(i + 1)))
            .GroupBy(x => x.Detalle.IdLocacion);

        var hoy = DateTime.Today;

        foreach (var grupo in grupos)
        {
            int idLocacion = grupo.Key;

            // 3a. NumDocumento: auto-incrementa el correlativo del documento
            int numDocumento = await GetNuevoCorrelativoDocumentoAsync(
                conn, tx, idSucursal, (int)EDocumento.GuiaInternaSalida, numSerie, cancellationToken);

            // 3b. insMovimientoalmacen
            long idMovimientoAlmacen = await InsMovimientoAlmacenAsync(
                conn, tx, idSucursal, idLocacion, idCliente,
                numSerie, numDocumento, hoy, cancellationToken);

            // 3c. Por cada artículo del grupo
            short corrDetMov = 1;
            foreach (var (detalle, correlativoVenta) in grupo)
            {
                decimal costoPromedio = await GetCostoPromedioAsync(
                    conn, tx, idLocacion, detalle.IdArticulo, cancellationToken);

                decimal stockAnterior = await GetStockAsync(
                    conn, tx, idLocacion, detalle.IdArticulo, detalle.IdUnidad, cancellationToken);

                decimal stockActual = stockAnterior - detalle.Cantidad;

                await InsDetalleMovimientoAlmacenAsync(
                    conn, tx, idMovimientoAlmacen, corrDetMov++, idLocacion, detalle,
                    costoPromedio, stockAnterior, stockActual, cancellationToken);

                await InsUpdArticuloStockAsync(
                    conn, tx, idLocacion, detalle.IdArticulo, detalle.IdUnidad,
                    stockActual, costoPromedio, cancellationToken);
            }

            // 3d. insGuiaRemision
            int idGuiaRemision = await InsGuiaRemisionAsync(
                conn, tx, idEmpresa, idSucursal, idLocacion, idCliente, idVenta,
                idVendedor, idTipoMoneda, importeTotal,
                numSerie, numDocumento, idMovimientoAlmacen,
                puntoPartida, hoy, cancellationToken);

            // 3e. InsDetalleGuiaRemision por cada artículo
            short corrGuia = 1;
            foreach (var (detalle, correlativoVenta) in grupo)
            {
                decimal costoPromedio = await GetCostoPromedioAsync(
                    conn, tx, idLocacion, detalle.IdArticulo, cancellationToken);

                await InsDetalleGuiaRemisionAsync(
                    conn, tx, idGuiaRemision, corrGuia++, correlativoVenta,
                    idEmpresa, detalle, costoPromedio, hoy, cancellationToken);
            }
        }
    }

    // ─── Helpers de lectura ───────────────────────────────────────────────────

    private static async Task<string> GetSucursalDireccionAsync(
        System.Data.Common.DbConnection conn,
        System.Data.Common.DbTransaction? tx,
        short idSucursal, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = "SELECT ISNULL(Direccion, '') FROM dbo.Sucursal WHERE IdSucursal = @s";
        var p = cmd.CreateParameter(); p.ParameterName = "@s"; p.Value = idSucursal;
        cmd.Parameters.Add(p);
        var result = await cmd.ExecuteScalarAsync(ct);
        return result as string ?? string.Empty;
    }

    private static async Task<short> GetNumSerieDocumentoAsync(
        System.Data.Common.DbConnection conn,
        System.Data.Common.DbTransaction? tx,
        short idEmpresa, short idSucursal, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
            SELECT TOP 1 ISNULL(NumSerie, 0)
            FROM dbo.CorrelativoDocumento
            WHERE IdTipoDocumento = @td AND IdSucursal = @s AND Estado = 'A'
            """;
        var p1 = cmd.CreateParameter(); p1.ParameterName = "@td"; p1.Value = (short)EDocumento.GuiaInternaSalida;
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@s";  p2.Value = idSucursal;
        cmd.Parameters.Add(p1); cmd.Parameters.Add(p2);
        var result = await cmd.ExecuteScalarAsync(ct);
        return result is DBNull || result is null ? (short)0 : Convert.ToInt16(result);
    }

    private static async Task<int> GetNuevoCorrelativoDocumentoAsync(
        System.Data.Common.DbConnection conn,
        System.Data.Common.DbTransaction? tx,
        short idSucursal, int idTipoDocumento, short numSerie, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "dbo.GetNuevoCorrelativoDocumento";
        var p1 = cmd.CreateParameter(); p1.ParameterName = "@IdSucursal";      p1.Value = idSucursal;
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@IdTipoDocumento"; p2.Value = (short)idTipoDocumento;
        var p3 = cmd.CreateParameter(); p3.ParameterName = "@NumSerie";        p3.Value = numSerie;
        var p4 = cmd.CreateParameter(); p4.ParameterName = "@NumSerieA";       p4.Value = string.Empty;
        cmd.Parameters.Add(p1); cmd.Parameters.Add(p2);
        cmd.Parameters.Add(p3); cmd.Parameters.Add(p4);
        var result = await cmd.ExecuteScalarAsync(ct);
        return result is DBNull || result is null ? 1 : Convert.ToInt32(result);
    }

    private static async Task<decimal> GetCostoPromedioAsync(
        System.Data.Common.DbConnection conn,
        System.Data.Common.DbTransaction? tx,
        int idLocacion, int idArticulo, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
            SELECT ISNULL(CostoPromedio, 0)
            FROM dbo.ArticuloLoc
            WHERE IdLocacion = @loc AND IdArticulo = @art
            """;
        var p1 = cmd.CreateParameter(); p1.ParameterName = "@loc"; p1.Value = idLocacion;
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@art"; p2.Value = idArticulo;
        cmd.Parameters.Add(p1); cmd.Parameters.Add(p2);
        var result = await cmd.ExecuteScalarAsync(ct);
        return result is DBNull || result is null ? 0m : Convert.ToDecimal(result);
    }

    private static async Task<decimal> GetStockAsync(
        System.Data.Common.DbConnection conn,
        System.Data.Common.DbTransaction? tx,
        int idLocacion, int idArticulo, short idUnidad, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = """
            SELECT ISNULL(Stock, 0)
            FROM dbo.ArticuloStock
            WHERE IdLocacion = @loc AND IdArticulo = @art AND IdUnidad = @un
            """;
        var p1 = cmd.CreateParameter(); p1.ParameterName = "@loc"; p1.Value = idLocacion;
        var p2 = cmd.CreateParameter(); p2.ParameterName = "@art"; p2.Value = idArticulo;
        var p3 = cmd.CreateParameter(); p3.ParameterName = "@un";  p3.Value = idUnidad;
        cmd.Parameters.Add(p1); cmd.Parameters.Add(p2); cmd.Parameters.Add(p3);
        var result = await cmd.ExecuteScalarAsync(ct);
        return result is DBNull || result is null ? 0m : Convert.ToDecimal(result);
    }

    // ─── Helpers de inserción ─────────────────────────────────────────────────

    private static async Task<long> InsMovimientoAlmacenAsync(
        System.Data.Common.DbConnection conn,
        System.Data.Common.DbTransaction? tx,
        short idSucursal, int idLocacion, int idCliente,
        short numSerie, int numDocumento, DateTime hoy, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "dbo.insMovimientoalmacen";

        void Add(string name, object value)
        { var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value; cmd.Parameters.Add(p); }

        Add("@IdTipoTransferencia", (int)ETipoTransferencia.Venta);
        Add("@IdTipoDocumento",     (short)EDocumento.GuiaInternaSalida);
        Add("@NumSerieT",           numSerie);
        Add("@NumDocumentoT",       numDocumento);
        Add("@IdSucursal",          (int)idSucursal);
        Add("@FechaDocumento",      hoy);
        Add("@FechaMovimiento",     hoy);
        Add("@EstadoTransaccion",   "A");
        Add("@Comentario",          string.Empty);
        Add("@IdEntidadRef",        idCliente);
        Add("@TipoEntidad",         (byte)ETipoEntidad.Cliente);
        Add("@IdLocacion",          idLocacion);
        Add("@IdInventario",        0);
        Add("@Usuario",             "admin");
        Add("@IdUsuario",           DBNull.Value);

        var outParam = cmd.CreateParameter();
        outParam.ParameterName = "@IdMovimientoAlmacen";
        outParam.Direction = System.Data.ParameterDirection.Output;
        outParam.DbType = System.Data.DbType.Int64;
        cmd.Parameters.Add(outParam);

        await cmd.ExecuteNonQueryAsync(ct);
        return outParam.Value is DBNull || outParam.Value is null ? 0 : Convert.ToInt64(outParam.Value);
    }

    private static async Task InsDetalleMovimientoAlmacenAsync(
        System.Data.Common.DbConnection conn,
        System.Data.Common.DbTransaction? tx,
        long idMovimientoAlmacen, short correlativo, int idLocacion,
        CreateVentaDetalleDto d, decimal costoPromedio,
        decimal stockAnterior, decimal stockActual, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "dbo.insDetallemovimientoalmacen";

        void Add(string name, object value)
        { var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value; cmd.Parameters.Add(p); }

        Add("@IdMovimientoAlmacen", idMovimientoAlmacen);
        Add("@Correlativo",         correlativo);
        Add("@CorrelativoRef",      (short)0);
        Add("@IdArticulo",          d.IdArticulo);
        Add("@IdLocacion",          idLocacion);
        Add("@IdUnidad",            d.IdUnidad);
        Add("@Cantidad",            d.Cantidad);
        Add("@StockAnterior",       stockAnterior);
        Add("@StockActual",         stockActual);
        Add("@CostoBase",           costoPromedio);
        Add("@CostoAdicional",      0m);
        Add("@CostoArticulo",       costoPromedio);
        Add("@CostoPromedio",       costoPromedio);
        Add("@IdUsuario",           DBNull.Value);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static async Task InsUpdArticuloStockAsync(
        System.Data.Common.DbConnection conn,
        System.Data.Common.DbTransaction? tx,
        int idLocacion, int idArticulo, short idUnidad,
        decimal stock, decimal costoPromedio, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "dbo.InsUpdArticuloStock";

        void Add(string name, object value)
        { var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value; cmd.Parameters.Add(p); }

        Add("@IdLocacion",    idLocacion);
        Add("@IdArticulo",    idArticulo);
        Add("@Stock",         stock);
        Add("@CostoPromedio", costoPromedio);
        Add("@IdUnidad",      idUnidad);

        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static async Task<int> InsGuiaRemisionAsync(
        System.Data.Common.DbConnection conn,
        System.Data.Common.DbTransaction? tx,
        short idEmpresa, short idSucursal, int idLocacion,
        int idCliente, int idVenta, short idVendedor,
        short idTipoMoneda, decimal importeTotal,
        short numSerie, int numDocumento, long idMovAlmacen,
        string puntoPartida, DateTime hoy, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "dbo.insGuiaRemision";

        void Add(string name, object value)
        { var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value; cmd.Parameters.Add(p); }

        Add("@IdEmpresa",          (int)idEmpresa);
        Add("@IdSucursal",         (int)idSucursal);
        Add("@NroSecuencia",       0);
        Add("@IdTipoDocumento",    (int)EDocumento.GuiaInternaSalida);
        Add("@NumSerie",           (int)numSerie);
        Add("@NumDocumento",       numDocumento);
        Add("@FechaTraslado",      hoy);
        Add("@FechaIngreso",       hoy);
        Add("@TipoEntidad",        (byte)ETipoEntidad.Cliente);
        Add("@IdCliente",          idCliente);
        Add("@IdLocalDespacho",    DBNull.Value);
        Add("@IdProveedor",        DBNull.Value);
        Add("@IdMotivoGuia",       (int)ETipoTransferencia.Venta);
        Add("@PuntoPartida",       puntoPartida);
        Add("@PuntoLlegada",       string.Empty);
        Add("@IdOrdenCompra",      DBNull.Value);
        Add("@IdPedido",           DBNull.Value);
        Add("@PesoTotal",          0m);
        Add("@VolumenTotal",       0m);
        Add("@IdMovAlmacen",       (int)idMovAlmacen);
        Add("@IdMovAlmacenTran",   DBNull.Value);
        Add("@Referencia",         string.Empty);
        Add("@Observacion",        string.Empty);
        Add("@Nota",               string.Empty);
        Add("@Estado",             "A");
        Add("@IdCompra",           DBNull.Value);
        Add("@IdVentaRef",         idVenta);
        Add("@IdGuiaRemisionRef",  DBNull.Value);
        Add("@IdTrabajador",       (int)idVendedor);
        Add("@IdVendedor",         (int)idVendedor);
        Add("@IdTipoMoneda",       (int)idTipoMoneda);
        Add("@ImporteTotal",       importeTotal);
        Add("@IdProceso",          0);
        Add("@TipoProceso",        (byte)0);
        Add("@IdProceso2",         idVenta);
        Add("@TipoProceso2",       (byte)ETipoProceso.Venta);
        Add("@IdProceso3",         0);
        Add("@TipoProceso3",       (byte)0);
        Add("@FlagDocumentoRef",   (byte)0);
        Add("@IdLocacion",         idLocacion);
        Add("@IdLocacionRef",      DBNull.Value);
        Add("@IdSucursalRef",      DBNull.Value);
        Add("@IdCentroCosto",      DBNull.Value);
        Add("@NumProyecto",        DBNull.Value);
        Add("@NombreReferencia",   string.Empty);
        Add("@UpdateToken",        (byte)0);
        Add("@Usuario",            "admin");
        Add("@CantidadSalida",     0m);
        Add("@CantidadMerma",      0m);
        Add("@FlagGratuito",       (byte)0);
        Add("@IdUsuario",          DBNull.Value);
        Add("@TipoEntidadRef",     DBNull.Value);
        Add("@IdClienteRef",       DBNull.Value);
        Add("@IdProveedorRef",     DBNull.Value);
        Add("@IdTrabajadorRef",    DBNull.Value);
        Add("@IdSucursalRef2",     DBNull.Value);
        Add("@IdAgenciaAduana",    DBNull.Value);
        Add("@NumSerieA",          string.Empty);
        Add("@NumDocumentoA",      string.Empty);
        Add("@IndicadorSunat",     string.Empty);
        Add("@Otros",              string.Empty);
        // @CodigoQR es varbinary(max) — necesita tipo explícito para evitar error de conversión nvarchar
        var pQR = cmd.CreateParameter();
        pQR.ParameterName = "@CodigoQR";
        pQR.DbType = System.Data.DbType.Binary;
        pQR.Value = DBNull.Value;
        cmd.Parameters.Add(pQR);

        var outIdGuia = cmd.CreateParameter();
        outIdGuia.ParameterName = "@IdGuiaRemision";
        outIdGuia.Direction = System.Data.ParameterDirection.Output;
        outIdGuia.DbType = System.Data.DbType.Int32;
        cmd.Parameters.Add(outIdGuia);

        var outNroSec = cmd.CreateParameter();
        outNroSec.ParameterName = "@NroSecuenciaGen";
        outNroSec.Direction = System.Data.ParameterDirection.Output;
        outNroSec.DbType = System.Data.DbType.Int32;
        cmd.Parameters.Add(outNroSec);

        await cmd.ExecuteNonQueryAsync(ct);
        return outIdGuia.Value is DBNull || outIdGuia.Value is null ? 0 : Convert.ToInt32(outIdGuia.Value);
    }

    private static async Task InsDetalleGuiaRemisionAsync(
        System.Data.Common.DbConnection conn,
        System.Data.Common.DbTransaction? tx,
        int idGuiaRemision, short correlativoGuia, short correlativoVenta,
        short idEmpresa, CreateVentaDetalleDto d,
        decimal costoPromedio, DateTime hoy, CancellationToken ct)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        cmd.CommandText = "dbo.InsDetalleGuiaRemision";

        void Add(string name, object value)
        { var p = cmd.CreateParameter(); p.ParameterName = name; p.Value = value; cmd.Parameters.Add(p); }

        Add("@IdGuiaRemision",          idGuiaRemision);
        Add("@CorrelativoGuia",         correlativoGuia);
        Add("@IdArticulo",              d.IdArticulo);
        Add("@IdUnidad",                d.IdUnidad);
        Add("@Cantidad",                d.Cantidad);
        Add("@CantidadRec",             0m);
        Add("@CostoBase",               costoPromedio);
        Add("@CostoAdicional",          0m);
        Add("@CostoUnd",                costoPromedio);
        Add("@Peso",                    0m);
        Add("@IGV",                     false);
        Add("@CorrelativoPedido",       DBNull.Value);
        Add("@CorrelativoDoc",          correlativoVenta);
        Add("@PrecioUnd",               d.PrecioUnitario);
        Add("@IdConcepto",              DBNull.Value);   // FK_DetalleGuiaRemision_Articulo — no aplica en POS
        Add("@PesoFinal",               DBNull.Value);
        Add("@IdSerie",                 DBNull.Value);
        Add("@Descripcion",             string.Empty);
        Add("@Observacion",             string.Empty);
        Add("@IdMotivoTransferencia",   DBNull.Value);   // FK a TablaMaestra — no aplica en POS
        Add("@CantidadTransferencia",   0m);
        Add("@TextoSerie",              DBNull.Value);
        Add("@Anio",                    DBNull.Value);   // FK compuesta (Anio, IdEmpresa, IdCentroCosto) — sin centro costo en POS
        Add("@IdEmpresa",               DBNull.Value);
        Add("@IdCentroCosto",           DBNull.Value);
        Add("@IdUsuario",               DBNull.Value);

        await cmd.ExecuteNonQueryAsync(ct);
    }
}
