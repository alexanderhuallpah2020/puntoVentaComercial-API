using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.FamiliasSunat
{
    public sealed class FamiliaSunat : Entity
    {
        private FamiliaSunat() { }

        public int IdFamiliaSunat => Id;

        public int IdSegmentoSunat { get; private set; }

        public string Codigo { get; private set; } = default!;
        public string Descripcion { get; private set; } = default!;
        public EEstado Estado { get; private set; }

        public short UpdateToken { get; private set; }

        public short IdUsuarioCreador { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public short? IdUsuarioModificador { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        public static Result<FamiliaSunat> Create(
            int id,
            int idSegmentoSunat,
            string codigo,
            string descripcion,
            short updateToken,
            short idUsuarioCreador,
            DateTime fechaCreacion)
        {
            var familia = new FamiliaSunat
            {
                Id = id,
                IdSegmentoSunat = idSegmentoSunat,
                Codigo = codigo.Trim().ToUpper(),
                Descripcion = descripcion.Trim(),
                Estado = EEstado.Activo,
                UpdateToken = updateToken,
                IdUsuarioCreador = idUsuarioCreador,
                FechaCreacion = fechaCreacion
            };

            return Result.Success(familia);
        }
    }
}
