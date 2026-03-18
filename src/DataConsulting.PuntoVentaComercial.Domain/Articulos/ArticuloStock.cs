namespace DataConsulting.PuntoVentaComercial.Domain.Articulos;

public sealed class ArticuloStock
{
    // PK compuesta: (IdLocacion, IdArticulo, IdUnidad) — sin IDENTITY
    public int IdLocacion { get; private set; }
    public int IdArticulo { get; private set; }
    public short IdUnidad { get; private set; }
    public decimal Stock { get; private set; }
    public decimal CostoPromedio { get; private set; }
    public short? IdUsuarioCreador { get; private set; }
    public DateTime? FechaCreacion { get; private set; }
    public short? IdUsuarioModificador { get; private set; }
    public DateTime? FechaModificacion { get; private set; }

    private ArticuloStock() { }

    /// <summary>Crea un nuevo registro de stock (primera vez que el artículo entra a la locación).</summary>
    public static ArticuloStock Create(
        int idLocacion,
        int idArticulo,
        short idUnidad,
        decimal stock,
        decimal costoPromedio)
    {
        return new ArticuloStock
        {
            IdLocacion    = idLocacion,
            IdArticulo    = idArticulo,
            IdUnidad      = idUnidad,
            Stock         = stock,
            CostoPromedio = costoPromedio,
            FechaCreacion = DateTime.Now,
        };
    }

    /// <summary>Actualiza stock y costo promedio (registro ya existente).</summary>
    public void Update(decimal nuevoStock, decimal nuevoCostoPromedio)
    {
        Stock                = nuevoStock;
        CostoPromedio        = nuevoCostoPromedio;
        FechaModificacion    = DateTime.Now;
    }
}
