using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Identity
{
    public sealed class User : Entity
    {
        public string Username { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public int IdEmpresa { get; private set; }
        public int IdTrabajador { get; private set; }
        public string NombreTrabajador { get; private set; } = string.Empty;
        public bool Activo { get; private set; }

        // ORM constructor
        private User() { }

        public User(
            int id,
            string username,
            string passwordHash,
            int idEmpresa,
            int idTrabajador,
            string nombreTrabajador,
            bool activo)
            : base(id)
        {
            Username = username;
            PasswordHash = passwordHash;
            IdEmpresa = idEmpresa;
            IdTrabajador = idTrabajador;
            NombreTrabajador = nombreTrabajador;
            Activo = activo;
        }
    }
}
