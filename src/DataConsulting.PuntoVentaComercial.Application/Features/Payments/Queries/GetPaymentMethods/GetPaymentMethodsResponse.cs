namespace DataConsulting.PuntoVentaComercial.Application.Features.Payments.Queries.GetPaymentMethods
{
    public sealed record GetPaymentMethodsResponse(IReadOnlyList<PaymentMethodDto> Items);

    public sealed record PaymentMethodDto(
        int IdFormaPago,
        string Descripcion,
        bool Activo);
}
