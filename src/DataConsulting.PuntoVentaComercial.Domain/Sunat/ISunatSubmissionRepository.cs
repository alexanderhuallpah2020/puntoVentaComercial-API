namespace DataConsulting.PuntoVentaComercial.Domain.Sunat
{
    public interface ISunatSubmissionRepository
    {
        Task<SunatSubmission?> GetByIdVentaAsync(int idVenta, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PendingSubmissionResult>> GetPendingAsync(
            int idEmpresa,
            int idSucursal,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int pageSize,
            CancellationToken cancellationToken = default);

        void Add(SunatSubmission submission);

        void Update(SunatSubmission submission);
    }

    public sealed record PendingSubmissionResult(
        int IdVenta,
        int IdEmpresa,
        int IdSucursal,
        DateTime FechaEmision,
        int TipoDocumento,
        string NumSerie,
        long Correlativo,
        string NombreCliente,
        string NumDocumentoCliente,
        decimal ImporteTotal,
        int? EstadoSunat,
        string? CodigoRespuesta,
        string? MensajeRespuesta,
        int Intentos);
}
