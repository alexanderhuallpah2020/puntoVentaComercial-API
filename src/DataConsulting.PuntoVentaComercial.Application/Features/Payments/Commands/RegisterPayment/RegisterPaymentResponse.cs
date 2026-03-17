namespace DataConsulting.PuntoVentaComercial.Application.Features.Payments.Commands.RegisterPayment
{
    public sealed record RegisterPaymentResponse(
        int IdOperacion,
        int IdVenta,
        decimal ImporteTotal,
        decimal ImportePagado,
        decimal Vuelto,
        decimal Credito);
}
