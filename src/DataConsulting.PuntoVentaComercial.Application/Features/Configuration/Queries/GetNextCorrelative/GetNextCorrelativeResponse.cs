namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetNextCorrelative
{
    public sealed record GetNextCorrelativeResponse(
        int TipoDocumento,
        string NumSerie,
        long Correlativo,
        string NumeroFormateado   // ej. "B001-000125"
    );
}
