namespace DataConsulting.PuntoVentaComercial.Domain.FamiliasSunat
{
    public interface IFamiliaSunatRepository
    {
        Task<bool> ExistsByCodigoAsync(int idSegmentoSunat, string codigo, CancellationToken cancellationToken = default);
        Task<int> GetNextIdAsync(CancellationToken cancellationToken = default);

        Task<FamiliaSunat?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        void Add(FamiliaSunat entity);
        void Remove(FamiliaSunat entity);
    }
}
