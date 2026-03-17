using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    public interface IDocumentSeriesRepository
    {
        Task<IReadOnlyList<DocumentSeries>> GetByEstacionAsync(
            int idEmpresa,
            int idSucursal,
            int idEstacion,
            CancellationToken cancellationToken = default);

        Task<long> GetNextCorrelativeAsync(
            int idEmpresa,
            int idSucursal,
            EDocumento tipoDocumento,
            string numSerie,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Incrementa atómicamente el correlativo y retorna el nuevo valor.
        /// Debe ejecutarse dentro de una transacción de base de datos.
        /// </summary>
        Task<long> IncrementAndGetCorrelativeAsync(
            int idEmpresa,
            int idSucursal,
            EDocumento tipoDocumento,
            string numSerie,
            CancellationToken cancellationToken = default);
    }
}
