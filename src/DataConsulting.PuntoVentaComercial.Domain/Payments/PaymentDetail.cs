using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Payments
{
    public sealed class PaymentDetail : Entity
    {
        // ORM constructor
        private PaymentDetail() { }

        public int IdOperacion { get; private set; }
        public int IdFormaPago { get; private set; }
        public string Descripcion { get; private set; } = string.Empty;
        public ETipoMoneda TipoMoneda { get; private set; }
        public decimal TipoCambio { get; private set; }
        public decimal Importe { get; private set; }
        public decimal ImporteEnSoles { get; private set; }

        public static PaymentDetail Create(
            int idOperacion,
            int idFormaPago,
            string descripcion,
            ETipoMoneda tipoMoneda,
            decimal tipoCambio,
            decimal importe)
        {
            decimal importeEnSoles = tipoMoneda == ETipoMoneda.Soles
                ? importe
                : Math.Round(importe * tipoCambio, 2);

            return new PaymentDetail
            {
                IdOperacion = idOperacion,
                IdFormaPago = idFormaPago,
                Descripcion = descripcion?.Trim() ?? string.Empty,
                TipoMoneda = tipoMoneda,
                TipoCambio = tipoCambio <= 0 ? 1m : tipoCambio,
                Importe = importe,
                ImporteEnSoles = importeEnSoles
            };
        }
    }
}
