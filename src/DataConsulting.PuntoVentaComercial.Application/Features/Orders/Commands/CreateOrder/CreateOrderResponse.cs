namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Commands.CreateOrder
{
    public sealed record CreateOrderResponse(
        int IdPedido,
        string NumSerie,
        long Correlativo,
        string NumeroFormateado,
        decimal ImporteTotal);
}
