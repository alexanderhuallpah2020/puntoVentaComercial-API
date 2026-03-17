using DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetPrintableOrder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Templates
{
    internal sealed class TicketPedidoTemplate(GetPrintableOrderResponse data) : IDocument
    {
        private const float AnchoTicket = 226.77f; // 80 mm en puntos

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(new PageSize(AnchoTicket, float.MaxValue));
                page.Margin(8, Unit.Point);
                page.DefaultTextStyle(style => style.FontFamily(Fonts.Arial).FontSize(8));

                page.Content().Column(col =>
                {
                    col.Spacing(3);

                    // ── Cabecera empresa ──────────────────────────────────────
                    if (!string.IsNullOrWhiteSpace(data.NombreEmpresa))
                        col.Item().AlignCenter().Text(data.NombreEmpresa).FontSize(10).Bold();
                    if (!string.IsNullOrWhiteSpace(data.RucEmpresa))
                        col.Item().AlignCenter().Text($"RUC: {data.RucEmpresa}");
                    if (!string.IsNullOrWhiteSpace(data.DireccionEmpresa))
                        col.Item().AlignCenter().Text(data.DireccionEmpresa).FontSize(7);

                    col.Item().PaddingTop(2).LineHorizontal(0.5f);

                    // ── Tipo de documento ─────────────────────────────────────
                    col.Item().AlignCenter().Text("PEDIDO").FontSize(11).Bold();
                    col.Item().AlignCenter().Text(data.NumeroFormateado).FontSize(9).Bold();
                    col.Item().Text($"Fecha: {data.FechaEmision:dd/MM/yyyy HH:mm}");

                    col.Item().LineHorizontal(0.5f);

                    // ── Cliente ───────────────────────────────────────────────
                    col.Item().Text($"Cliente : {data.NombreCliente}");
                    if (!string.IsNullOrWhiteSpace(data.NumDocumentoCliente))
                        col.Item().Text($"Doc.    : {data.NumDocumentoCliente}");
                    if (!string.IsNullOrWhiteSpace(data.DireccionCliente))
                        col.Item().Text($"Dir.    : {data.DireccionCliente}").FontSize(7);

                    col.Item().LineHorizontal(0.5f);

                    // ── Ítems ─────────────────────────────────────────────────
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(4);
                            c.RelativeColumn(1.2f);
                            c.RelativeColumn(1.6f);
                            c.RelativeColumn(1.6f);
                        });

                        table.Header(h =>
                        {
                            h.Cell().Text("Descripción").Bold();
                            h.Cell().AlignCenter().Text("Cant").Bold();
                            h.Cell().AlignRight().Text("P.U.").Bold();
                            h.Cell().AlignRight().Text("Total").Bold();
                        });

                        foreach (var item in data.Items)
                        {
                            table.Cell().Text($"[{item.Codigo}] {item.Descripcion}").FontSize(7);
                            table.Cell().AlignCenter().Text($"{item.Cantidad:0.##} {item.SiglaUnidad}").FontSize(7);
                            table.Cell().AlignRight().Text($"{item.PrecioUnitario:N2}").FontSize(7);
                            table.Cell().AlignRight().Text($"{item.Subtotal:N2}").FontSize(7);
                        }
                    });

                    col.Item().LineHorizontal(0.5f);

                    // ── Totales ───────────────────────────────────────────────
                    if (data.DescuentoGlobal > 0)
                        TotalRow(col, "Desc. Global:", -data.DescuentoGlobal);

                    col.Item().PaddingTop(1).Row(row =>
                    {
                        row.RelativeItem().Text("TOTAL:").FontSize(10).Bold();
                        row.ConstantItem(65).AlignRight()
                            .Text($"S/ {data.ImporteTotal:N2}").FontSize(10).Bold();
                    });

                    // ── Observaciones ─────────────────────────────────────────
                    if (!string.IsNullOrWhiteSpace(data.Observaciones))
                    {
                        col.Item().PaddingTop(2).LineHorizontal(0.5f);
                        col.Item().Text($"Obs: {data.Observaciones}").Italic().FontSize(7);
                    }

                    col.Item().PaddingTop(3).LineHorizontal(0.5f);
                    col.Item().AlignCenter().Text("Documento no válido como comprobante de pago").Italic().FontSize(7);
                });
            });
        }

        private static void TotalRow(ColumnDescriptor col, string label, decimal valor)
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Text(label);
                row.ConstantItem(65).AlignRight().Text($"{valor:N2}");
            });
        }
    }
}
