using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    public sealed class ClientLocal : Entity
    {
        private ClientLocal() { }

        public int IdLocal => Id;
        public int IdCliente { get; private set; }
        public string DireccionLocal { get; private set; } = default!;
        public string Telefono1 { get; private set; } = default!;
        public int IdTipoCliente { get; private set; }
        public int IdSucursal { get; private set; }
        public int? IdRuta { get; private set; }
        public EEstado Estado { get; private set; }
    }
}
