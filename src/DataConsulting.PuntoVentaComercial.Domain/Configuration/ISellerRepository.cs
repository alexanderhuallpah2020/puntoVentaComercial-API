namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    public interface ISellerRepository
    {
        Task<IReadOnlyList<Seller>> GetActiveSellersAsync(
            int idEmpresa,
            CancellationToken cancellationToken = default);
    }
}
