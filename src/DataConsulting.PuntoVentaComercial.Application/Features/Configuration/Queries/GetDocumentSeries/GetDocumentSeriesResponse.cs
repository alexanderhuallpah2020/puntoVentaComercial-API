namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetDocumentSeries
{
    public sealed record GetDocumentSeriesResponse(IReadOnlyList<DocumentSeriesDto> Series);

    public sealed record DocumentSeriesDto(
        int IdEmpresa,
        int IdSucursal,
        int IdEstacion,
        int TipoDocumento,
        string DescripcionTipoDoc,
        string NumSerie,
        long UltimoCorrelativo
    );
}
