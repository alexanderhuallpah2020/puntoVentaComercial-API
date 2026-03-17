using DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetPrintableOrder;
using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetPrintableSale;

namespace DataConsulting.PuntoVentaComercial.Application.Services.Print
{
    public interface IPrintService
    {
        byte[] GenerateSalePdf(GetPrintableSaleResponse data);
        byte[] GenerateOrderPdf(GetPrintableOrderResponse data);
    }
}
