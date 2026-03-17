namespace DataConsulting.PuntoVentaComercial.Domain.Sales
{
    public interface ISaleRepository
    {
        Task<Sale?> GetByIdAsync(int idVenta, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<SaleSearchResult>> SearchAsync(
            int idEmpresa,
            int idSucursal,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int? idTipoDocumento,
            string? numSerie,
            long? correlativo,
            int? idCliente,
            int? estado,
            int pageSize,
            CancellationToken cancellationToken = default);

        void Add(Sale sale);

        void Update(Sale sale);
    }

    public sealed record SaleSearchResult(
        int IdVenta,
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
