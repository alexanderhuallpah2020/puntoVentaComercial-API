using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    public sealed class ExchangeRate : Entity
    {
        public int IdEmpresa { get; private set; }
        public DateOnly Fecha { get; private set; }
        public ETipoMoneda TipoMoneda { get; private set; }
        public decimal TipoCambioCompra { get; private set; }
        public decimal TipoCambioVenta { get; private set; }

        private ExchangeRate() { }
    }
}
