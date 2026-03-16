using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    public sealed class Currency : Entity
    {
        public ETipoMoneda TipoMoneda { get; private set; }
        public string Codigo { get; private set; } = string.Empty;      // PEN, USD, EUR
        public string Simbolo { get; private set; } = string.Empty;     // S/, $, €
        public string Descripcion { get; private set; } = string.Empty;
        public bool Activo { get; private set; }

        private Currency() { }
    }
}
