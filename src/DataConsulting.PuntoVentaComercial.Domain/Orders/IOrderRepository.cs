namespace DataConsulting.PuntoVentaComercial.Domain.Orders
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int idPedido, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<OrderSearchResult>> SearchAsync(
            int idEmpresa,
            int idSucursal,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int? idCliente,
            int? idTrabajador,
            int? estado,
            int pageSize,
            CancellationToken cancellationToken = default);

        void Add(Order order);

        void Update(Order order);
    }

    public sealed record OrderSearchResult(
        int IdPedido,
        int IdEmpresa,
        int IdSucursal,
        DateTime FechaEmision,
        int TipoDocumento,
        string NumSerie,
        long Correlativo,
        int IdCliente,
        string NombreCliente,
        string NumDocumentoCliente,
        decimal ImporteTotal,
        int Estado);
}
