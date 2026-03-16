namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetAllSegmentosSunat
{
    public sealed class GetAllSegmentosSunatResponse
    {
        public int IdSegmentoSunat { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public string Descripcion { get; init; } = string.Empty;

        public short Estado { get; init; }
        public short UpdateToken { get; init; }

        public short IdUsuarioCreador { get; init; }
        public DateTime FechaCreacion { get; init; }
        public short? IdUsuarioModificador { get; init; }
        public DateTime? FechaModificacion { get; init; }
    }
}
