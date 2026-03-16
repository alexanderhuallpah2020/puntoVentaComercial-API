using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    public sealed class Shift : Entity
    {
        public int IdEmpresa { get; private set; }
        public string Descripcion { get; private set; } = string.Empty;
        public TimeOnly HoraInicio { get; private set; }
        public TimeOnly HoraFin { get; private set; }
        public bool Activo { get; private set; }

        private Shift() { }
    }
}
