using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.SegmentosSunat
{
    public sealed class SegmentoSunat : Entity
    {
        private SegmentoSunat() { }

        public int IdSegmentoSunat => Id;
        public string Codigo { get; private set; } = default!;
        public string Descripcion { get; private set; } = default!;
        public EEstado Estado { get; private set; }

        // Concurrencia (en su tabla es smallint)
        public short UpdateToken { get; private set; }

        // Auditoría (dominio puro: no depende de infraestructura)
        public short IdUsuarioCreador { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public short? IdUsuarioModificador { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        public static Result<SegmentoSunat> Create(
            int id,
            string codigo,
            string descripcion,
            short updateToken,
            short idUsuarioCreador,
            DateTime fechaCreacion)
        {

            var segmento = new SegmentoSunat
            {
                Id = id,
                Codigo = codigo.Trim().ToUpper(),
                Descripcion = descripcion.Trim(),
                Estado = EEstado.Activo,
                UpdateToken = updateToken,
                IdUsuarioCreador = idUsuarioCreador,
                FechaCreacion = fechaCreacion
            };

            return Result.Success(segmento);
        }
    }
}