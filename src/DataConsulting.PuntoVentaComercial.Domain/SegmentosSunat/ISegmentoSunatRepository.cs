namespace DataConsulting.PuntoVentaComercial.Domain.SegmentosSunat
{
    public interface ISegmentoSunatRepository
    {
        Task<SegmentoSunat?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        void Add(SegmentoSunat entity);
        Task<IReadOnlyList<SegmentoSunat>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodigoAsync(string codigo, CancellationToken cancellationToken = default);
        Task<int> GetNextIdAsync(CancellationToken cancellationToken = default);
    }
}
