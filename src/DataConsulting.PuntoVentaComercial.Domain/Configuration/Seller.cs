using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    public sealed class Seller : Entity
    {
        public int IdEmpresa { get; private set; }
        public string Nombres { get; private set; } = string.Empty;
        public string Apellidos { get; private set; } = string.Empty;
        public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
        public string? Codigo { get; private set; }
        public decimal PorcentajeDescuentoMaximo { get; private set; }
        public bool Activo { get; private set; }

        private Seller() { }
    }
}
