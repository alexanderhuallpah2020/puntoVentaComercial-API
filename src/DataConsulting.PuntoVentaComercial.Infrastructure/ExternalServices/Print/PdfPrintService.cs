using DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetPrintableOrder;
using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetPrintableSale;
using DataConsulting.PuntoVentaComercial.Application.Services.Print;
using DataConsulting.PuntoVentaComercial.Infrastructure.Templates;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.ExternalServices.Print
{
    /// <summary>
    /// Genera PDFs de tickets usando QuestPDF (licencia Community).
    /// Para uso comercial con ingresos mayores a USD 1M se requiere licencia profesional.
    /// </summary>
    internal sealed class PdfPrintService : IPrintService
    {
        static PdfPrintService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateSalePdf(GetPrintableSaleResponse data)
        {
            return new TicketVentaTemplate(data).GeneratePdf();
        }

        public byte[] GenerateOrderPdf(GetPrintableOrderResponse data)
        {
            return new TicketPedidoTemplate(data).GeneratePdf();
        }
    }
}
