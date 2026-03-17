namespace DataConsulting.PuntoVentaComercial.Domain.Payments
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdVentaAsync(int idVenta, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentMethodResult>> GetPaymentMethodsAsync(
            int idEmpresa,
            CancellationToken cancellationToken = default);

        void Add(Payment payment);
    }

    public sealed record PaymentMethodResult(
        int IdFormaPago,
        string Descripcion,
        bool Activo);
}
