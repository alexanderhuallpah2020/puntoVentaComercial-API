using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Payments
{
    public sealed class Payment : Entity
    {
        private readonly List<PaymentDetail> _detalles = [];

        // ORM constructor
        private Payment() { }

        public int IdOperacion => Id;
        public int IdVenta { get; private set; }
        public int IdEmpresa { get; private set; }
        public int IdSucursal { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public int IdCliente { get; private set; }
        public decimal ImporteTotal { get; private set; }
        public decimal ImportePagado { get; private set; }
        public decimal Vuelto { get; private set; }
        public decimal Credito { get; private set; }
        public int Estado { get; private set; } = 1;
        public int IdUsuarioCreador { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        public IReadOnlyCollection<PaymentDetail> Detalles => _detalles.AsReadOnly();

        public static Payment Create(
            int idVenta,
            int idEmpresa,
            int idSucursal,
            int idCliente,
            decimal importeTotal,
            decimal importePagado,
            int idUsuarioCreador,
            DateTime fechaCreacion)
        {
            decimal vuelto = Math.Max(importePagado - importeTotal, 0m);
            decimal credito = importeTotal > importePagado ? importeTotal - importePagado : 0m;

            return new Payment
            {
                IdVenta = idVenta,
                IdEmpresa = idEmpresa,
                IdSucursal = idSucursal,
                FechaRegistro = fechaCreacion,
                IdCliente = idCliente,
                ImporteTotal = importeTotal,
                ImportePagado = importePagado,
                Vuelto = vuelto,
                Credito = credito,
                Estado = 1,
                IdUsuarioCreador = idUsuarioCreador,
                FechaCreacion = fechaCreacion
            };
        }

        public void AddDetalle(PaymentDetail detalle)
        {
            _detalles.Add(detalle);
        }
    }
}
