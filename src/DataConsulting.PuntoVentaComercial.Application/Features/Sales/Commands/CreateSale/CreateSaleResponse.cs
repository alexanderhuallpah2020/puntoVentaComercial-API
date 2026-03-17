namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CreateSale
{
    public sealed record CreateSaleResponse(
        int IdVenta,
        string NumSerie,
        long Correlativo,
        string NumeroFormateado,
        decimal ImporteTotal);
}
