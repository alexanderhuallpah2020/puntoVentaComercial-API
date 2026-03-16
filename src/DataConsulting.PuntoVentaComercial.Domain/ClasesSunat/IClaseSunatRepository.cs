namespace DataConsulting.PuntoVentaComercial.Domain.ClasesSunat
{
    public interface IClaseSunatRepository
    {
        Task<bool> ExistsByCodigoAsync(int idFamiliaSunat, string codigo, CancellationToken cancellationToken = default);

        Task<int> GetNextIdAsync(CancellationToken cancellationToken = default);

        Task<ClaseSunat?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        void Add(ClaseSunat entity);
        void Remove(ClaseSunat entity);
    }
}
