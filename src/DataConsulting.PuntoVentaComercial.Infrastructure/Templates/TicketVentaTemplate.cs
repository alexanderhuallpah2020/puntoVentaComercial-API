using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetPrintableSale;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Templates
{
    internal sealed class TicketVentaTemplate(GetPrintableSaleResponse data) : IDocument
    {
        // 80 mm en puntos (1 pulgada = 72 pt; 80 mm = 226.77 pt)
        private const float AnchoTicket = 226.77f;

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
                    {
                        col.Item().AlignCenter().Text(data.NombreEmpresa)
                            .FontSize(10).Bold();
                    }
                    if (!string.IsNullOrWhiteSpace(data.RucEmpresa))
                    {
                        col.Item().AlignCenter().Text($"RUC: {data.RucEmpresa}");
                    }
                    if (!string.IsNullOrWhiteSpace(data.DireccionEmpresa))
                    {
                        col.Item().AlignCenter().Text(data.DireccionEmpresa).FontSize(7);
                    }

                    col.Item().PaddingTop(2).LineHorizontal(0.5f);

                    // ── Tipo de documento y número ────────────────────────────
                    col.Item().AlignCenter().Text(GetDocumentoLabel(data.TipoDocumento))
                        .FontSize(9).Bold();
                    col.Item().AlignCenter().Text(data.NumeroFormateado)
                        .FontSize(9).Bold();
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
                            c.RelativeColumn(4);   // descripción
                            c.RelativeColumn(1.2f); // cant
                            c.RelativeColumn(1.6f); // precio
                            c.RelativeColumn(1.6f); // subtotal
                        });

                        // cabecera de tabla
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
                    if (data.FlagIGV)
                    {
                        if (data.ValorAfecto > 0)
                            TotalRow(col, "Op. Gravadas:", data.ValorAfecto);
                        if (data.ValorInafecto > 0)
                            TotalRow(col, "Op. Inafectas:", data.ValorInafecto);
                        if (data.ValorExonerado > 0)
                            TotalRow(col, "Op. Exoneradas:", data.ValorExonerado);
                        if (data.ValorGratuito > 0)
                            TotalRow(col, "Op. Gratuitas:", data.ValorGratuito);
                        if (data.TotalIsc > 0)
                            TotalRow(col, "ISC:", data.TotalIsc);
                        if (data.Igv > 0)
                            TotalRow(col, "IGV (18%):", data.Igv);
                        if (data.TotalIcbper > 0)
                            TotalRow(col, "ICBPER:", data.TotalIcbper);
                        if (data.DescuentoGlobal > 0)
                            TotalRow(col, "Desc. Global:", -data.DescuentoGlobal);
                    }

                    col.Item().PaddingTop(1).Row(row =>
                    {
                        row.RelativeItem().Text("TOTAL A PAGAR:").FontSize(10).Bold();
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
                    col.Item().AlignCenter().Text("¡Gracias por su preferencia!").Italic();
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

        private static string GetDocumentoLabel(EDocumento tipo) => tipo switch
        {
            EDocumento.Factura or EDocumento.FacturaElectronica => "FACTURA ELECTRÓNICA",
            EDocumento.Boleta or EDocumento.BoletaElectronica => "BOLETA DE VENTA ELECTRÓNICA",
            EDocumento.TicketBoleta or EDocumento.TicketFactura
                or EDocumento.TicketMaquinaRegistradora => "TICKET",
            EDocumento.NotaCredito or EDocumento.NotaCreditoElectronica => "NOTA DE CRÉDITO",
            EDocumento.NotaDebito or EDocumento.NotaDebitoElectronica => "NOTA DE DÉBITO",
            _ => tipo.ToString().ToUpperInvariant()
        };
    }
}
