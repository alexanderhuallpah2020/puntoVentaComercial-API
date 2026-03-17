namespace DataConsulting.PuntoVentaComercial.Domain.Ventas;

public interface IVentaRepository
{
    Task<Venta?> GetByIdAsync(int idVenta, CancellationToken ct);
    Task<(IList<Venta> Items, int Total)> SearchAsync(
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int? idCliente,
        short? numSerie,
        short? idTipoDocumento,
        string? estado,
        short? idSucursal,
        int page,
        int pageSize,
        CancellationToken ct);
    Task<int> GetNextNumeroDocumentoAsync(
        short idSucursal, short idTipoDocumento, short numSerie, CancellationToken ct);
    void Add(Venta venta);
}
