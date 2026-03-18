namespace DataConsulting.PuntoVentaComercial.Domain.MovimientosAlmacen;

public sealed class DetalleMovimientoAlmacen
{
    // PK compuesta: (IdMovimientoAlmacen, Correlativo)
    public long IdMovimientoAlmacen { get; private set; }
    public short Correlativo { get; private set; }
    public short CorrelativoRef { get; private set; }
    public int IdArticulo { get; private set; }
    public int IdLocacion { get; private set; }         // columna: idLocacion (minúscula)
    public short IdUnidad { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal CostoBase { get; private set; }
    public decimal CostoAdicional { get; private set; }
    public decimal CostoArticulo { get; private set; }
    public decimal CostoPromedio { get; private set; }
    public decimal CostoUtilidadSinIGV { get; private set; }
    public short? IdUsuarioCreador { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    // Nota: @StockAnterior y @StockActual del SP no se insertan en la tabla —
    // el SP los recibía pero no los usaba. El stock real se actualiza en ArticuloStock.

    private DetalleMovimientoAlmacen() { }

    public static DetalleMovimientoAlmacen Create(
        short correlativo,
        short correlativoRef,
        int idArticulo,
        int idLocacion,
        short idUnidad,
        decimal cantidad,
        decimal costoBase,
        decimal costoAdicional,
        decimal costoArticulo,
        decimal costoPromedio)
    {
        return new DetalleMovimientoAlmacen
        {
            Correlativo          = correlativo,
            CorrelativoRef       = correlativoRef,
            IdArticulo           = idArticulo,
            IdLocacion           = idLocacion,
            IdUnidad             = idUnidad,
            Cantidad             = cantidad,
            CostoBase            = costoBase,
            CostoAdicional       = costoAdicional,
            CostoArticulo        = costoArticulo,
            CostoPromedio        = costoPromedio,
            CostoUtilidadSinIGV  = 0m,
            FechaCreacion        = DateTime.Now,
        };
    }
}
