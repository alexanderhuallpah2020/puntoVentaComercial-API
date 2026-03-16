using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.ClasesSunat
{
    public sealed class ClaseSunat : Entity
    {
        private ClaseSunat() { }

        public int IdClaseSunat => Id;

        public int IdFamiliaSunat { get; private set; }

        public string Codigo { get; private set; } = default!;
        public string Descripcion { get; private set; } = default!;
        public EEstado Estado { get; private set; }

        public short UpdateToken { get; private set; }

        public short IdUsuarioCreador { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public short? IdUsuarioModificador { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        public static Result<ClaseSunat> Create(
            int id,
            int idFamiliaSunat,
            string codigo,
            string descripcion,
            short updateToken,
            short idUsuarioCreador,
            DateTime fechaCreacion)
        {
            var clase = new ClaseSunat
            {
                Id = id,
                IdFamiliaSunat = idFamiliaSunat,
                Codigo = codigo.Trim().ToUpper(),
                Descripcion = descripcion.Trim(),
                Estado = EEstado.Activo,
                UpdateToken = updateToken,
                IdUsuarioCreador = idUsuarioCreador,
                FechaCreacion = fechaCreacion
            };

            return Result.Success(clase);
        }
    }
}
