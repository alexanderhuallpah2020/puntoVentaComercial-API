namespace DataConsulting.PuntoVentaComercial.API.Controllers.ClasesSunat
{
    public sealed class CreateClaseSunatRequest
    {
        public int IdFamiliaSunat { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public string Descripcion { get; init; } = string.Empty;
    }
}
