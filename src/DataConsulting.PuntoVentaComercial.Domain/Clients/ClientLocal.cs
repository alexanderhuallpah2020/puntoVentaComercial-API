using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    /// <summary>
    /// Local (dirección operativa) del cliente para una sucursal específica.
    /// Corresponde a LocalClienteBE del legacy.
    /// </summary>
    public sealed class ClientLocal : Entity
    {
        // ORM constructor
        private ClientLocal() { }

        public int IdLocal => Id;
        public int IdCliente { get; private set; }
        public int IdSucursal { get; private set; }
        public string DireccionLocal { get; private set; } = string.Empty;
        public string Telefono1 { get; private set; } = string.Empty;
        public int IdTipoCliente { get; private set; }
        public string Estado { get; private set; } = "A";

        public static ClientLocal Create(
            int idCliente,
            int idSucursal,
            string direccion,
            string telefono,
            int idTipoCliente = 1)
        {
            return new ClientLocal
            {
                IdCliente = idCliente,
                IdSucursal = idSucursal,
                DireccionLocal = direccion?.Trim() ?? string.Empty,
                Telefono1 = telefono?.Trim() ?? string.Empty,
                IdTipoCliente = idTipoCliente,
                Estado = "A"
            };
        }

        internal void Update(string direccion, string telefono)
        {
            DireccionLocal = direccion?.Trim() ?? string.Empty;
            Telefono1 = telefono?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Reconstruye un ClientLocal desde una lectura Dapper (sin pasar por EF).
        /// </summary>
        public static ClientLocal Reconstruct(
            int id,
            int idCliente,
            int idSucursal,
            string direccion,
            string telefono,
            int idTipoCliente,
            string estado)
        {
            return new ClientLocal
            {
                Id = id,
                IdCliente = idCliente,
                IdSucursal = idSucursal,
                DireccionLocal = direccion ?? string.Empty,
                Telefono1 = telefono ?? string.Empty,
                IdTipoCliente = idTipoCliente,
                Estado = estado ?? "A"
            };
        }
    }
}
