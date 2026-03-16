using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CalculateSale;
using DataConsulting.PuntoVentaComercial.Domain.Constants;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Services.Sales
{
    internal sealed class SaleCalculationService : ISaleCalculationService
    {
        public CalculateSaleResponse Calculate(CalculateSaleCommand command)
        {
            var itemResponses = command.Items.Select(CalculateItem).ToList();

            decimal totalAfecto = itemResponses.Sum(i => i.ValorAfectoItem);
            decimal totalInafecto = itemResponses
                .Where(i => i.TipoAfectacionIgv == ETipoAfectacionIgv.Inafecto)
                .Sum(i => i.ValorItem);
            decimal totalExonerado = itemResponses
                .Where(i => i.TipoAfectacionIgv == ETipoAfectacionIgv.Exonerado)
                .Sum(i => i.ValorItem);
            decimal totalGratuito = itemResponses
                .Where(i => IsGratuito(i.TipoAfectacionIgv))
                .Sum(i => i.ValorItem);

            decimal descuentoGlobal = command.DescuentoGlobal;
            decimal baseImponible = Math.Max(totalAfecto - descuentoGlobal, 0m);

            decimal igv = Math.Round(baseImponible * TaxConstants.TasaIgv, 2);
            decimal isc = itemResponses.Sum(i => i.IscItem);
            decimal icbper = itemResponses.Sum(i => i.IcbperItem);

            decimal total = baseImponible + totalInafecto + totalExonerado + igv + isc + icbper;

            EDocumento tipoDocumento = command.TipoDocumentoCliente == EDocumentoIdentidad.RUC
                ? EDocumento.Factura
                : EDocumento.Boleta;

            return new CalculateSaleResponse(
                TipoDocumento: tipoDocumento,
                ValorAfecto: Math.Round(totalAfecto, 2),
                ValorInafecto: Math.Round(totalInafecto, 2),
                ValorExonerado: Math.Round(totalExonerado, 2),
                ValorGratuito: Math.Round(totalGratuito, 2),
                DescuentoGlobal: Math.Round(descuentoGlobal, 2),
                BaseImponible: Math.Round(baseImponible, 2),
                Igv: igv,
                Isc: Math.Round(isc, 2),
                Icbper: Math.Round(icbper, 2),
                Total: Math.Round(total, 2),
                Items: itemResponses);
        }

        private static CalculateSaleItemResponse CalculateItem(CalculateSaleItemCommand item)
        {
            decimal valorItem = Math.Round(item.Cantidad * item.PrecioUnitario, 2);

            bool esAfecto = item.TipoAfectacionIgv == ETipoAfectacionIgv.Gravado;
            decimal valorAfectoItem = esAfecto ? valorItem : 0m;

            decimal iscItem = CalculateIsc(item, valorAfectoItem);
            decimal icbperItem = item.EsBolsaPlastica
                ? Math.Round(item.Cantidad * TaxConstants.TarifaIcbper, 2)
                : 0m;

            decimal igvItem = esAfecto
                ? Math.Round(valorAfectoItem * TaxConstants.TasaIgv, 2)
                : 0m;

            decimal totalItem = valorAfectoItem + igvItem + iscItem + icbperItem
                + (esAfecto ? 0m : valorItem);

            return new CalculateSaleItemResponse(
                ProductoId: item.ProductoId,
                Cantidad: item.Cantidad,
                PrecioUnitario: item.PrecioUnitario,
                TipoAfectacionIgv: item.TipoAfectacionIgv,
                ValorItem: valorItem,
                DescuentoItem: 0m,
                ValorAfectoItem: valorAfectoItem,
                IscItem: Math.Round(iscItem, 2),
                IcbperItem: icbperItem,
                IgvItem: igvItem,
                TotalItem: Math.Round(totalItem, 2));
        }

        private static decimal CalculateIsc(CalculateSaleItemCommand item, decimal valorAfecto)
        {
            if (item.TipoIsc is null) return 0m;

            return item.TipoIsc switch
            {
                // Sistema 1: Al Valor — ISC = ValorAfecto * Tasa
                ETipoIsc.AlValor =>
                    item.TasaIsc.HasValue ? valorAfecto * item.TasaIsc.Value : 0m,

                // Sistema 2: Específico — ISC = Cantidad * MontoFijo
                ETipoIsc.Especifico =>
                    item.MontoFijoIsc.HasValue ? item.Cantidad * item.MontoFijoIsc.Value : 0m,

                // Sistema 3: Al Precio de Venta — ISC = (ValorAfecto * FactorIGV) * Tasa
                ETipoIsc.AlPrecioDeVenta =>
                    item.TasaIsc.HasValue
                        ? valorAfecto * TaxConstants.FactorIgv * item.TasaIsc.Value
                        : 0m,

                _ => 0m
            };
        }

        private static bool IsGratuito(ETipoAfectacionIgv tipo) =>
            tipo is ETipoAfectacionIgv.Gratuito
                or ETipoAfectacionIgv.GravadoRetiro
                or ETipoAfectacionIgv.GravadoMerma
                or ETipoAfectacionIgv.GravadoConsumo
                or ETipoAfectacionIgv.GravadoBonificacion;
    }
}
