namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase
{
    public sealed class GetSegmentoFamiliaClaseResponse
    {
        public string Segmento { get; init; } = string.Empty;
        public string SegmentoDescripcion { get; init; } = string.Empty;

        public string Familia { get; init; } = string.Empty;
        public string FamiliaDescripcion { get; init; } = string.Empty;

        public string Clase { get; init; } = string.Empty;
        public string ClaseDescripcion { get; init; } = string.Empty;
    }
}
