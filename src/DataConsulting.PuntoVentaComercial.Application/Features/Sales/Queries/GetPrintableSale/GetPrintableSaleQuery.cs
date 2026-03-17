using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetPrintableSale
{
    public sealed record GetPrintableSaleQuery(int IdVenta) : IQuery<GetPrintableSaleResponse>;
}
