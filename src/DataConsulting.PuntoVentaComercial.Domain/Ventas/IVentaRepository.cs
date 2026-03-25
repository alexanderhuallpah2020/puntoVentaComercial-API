namespace DataConsulting.PuntoVentaComercial.Domain.Ventas;

public interface IVentaRepository
{
    Task<Venta?> GetByIdAsync(int idVenta, CancellationToken ct);
    Task<(IList<VentaSearchResultDto> Items, int Total)> SearchAsync(
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        string? nombreCliente,
        string? numSerieA,
        int? numDocumento,
        short? idTipoDocumento,
        string? estado,
        int page,
        int pageSize,
        CancellationToken ct);
    Task<int?> GetNextNumeroDocumentoAsync(
        short idSucursal, short idTipoDocumento, string numSerieA, CancellationToken ct);
    // Correlativo contable mensual — solo cuando IdSubdiario > 0
    Task<int> GetNroCorrelativoVentaAsync(
        DateTime fechaEmision, short idSubdiario, CancellationToken ct);
    void Add(Venta venta);

    // SUNAT electronic billing
    /// <summary>
    /// Una sola consulta que retorna:
    ///   CodigoSunatRespuesta — respuesta previa de SUNAT en la venta ("0" = aceptada, null = nunca enviada).
    ///   EstadoSunat          — descripción de la respuesta previa.
    ///   CodigoSunatDocumento — código catálogo 01 del tipo de documento ("01"=Factura, "03"=Boleta, etc.).
    /// </summary>
    Task<(string? CodigoSunatRespuesta, string? EstadoSunat, string? CodigoSunatDocumento)>
        BuscarCodigoSunatVentaAsync(int idVenta, CancellationToken ct);
    Task InsVentaXmlLogAsync(int idVenta, string nombreXml, string nombreZip, int resultado, CancellationToken ct);
    Task UpdVentaArchivoXmlAsync(int idVenta, byte[] xmlBytes, string nombreXml, byte[]? cdrBytes, string? nombreCdr, string usuario, CancellationToken ct);
    Task UpdEstadoFacturaElectronicaAsync(int idVenta, string codigoSunat, string estadoSunat, string estado, CancellationToken ct);
}
