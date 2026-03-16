using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Identity
{
    public sealed class Workstation : Entity
    {
        public string Codigo { get; private set; } = string.Empty;
        public int IdSucursal { get; private set; }
        public string NombreSucursal { get; private set; } = string.Empty;
        public int IdEmpresa { get; private set; }
        public bool Activo { get; private set; }

        private Workstation() { }

        public Workstation(
            int id,
            string codigo,
            int idSucursal,
            string nombreSucursal,
            int idEmpresa,
            bool activo)
            : base(id)
        {
            Codigo = codigo;
            IdSucursal = idSucursal;
            NombreSucursal = nombreSucursal;
            IdEmpresa = idEmpresa;
            Activo = activo;
        }
    }
}
