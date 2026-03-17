using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Configuration;
using DataConsulting.PuntoVentaComercial.Domain.Sales;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CreateSale
{
    internal sealed class CreateSaleCommandHandler(
        ISaleRepository saleRepository,
        IDocumentSeriesRepository documentSeriesRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<CreateSaleCommand, CreateSaleResponse>
    {
        public async Task<Result<CreateSaleResponse>> Handle(
            CreateSaleCommand request,
            CancellationToken cancellationToken)
        {
            var now = DateTime.Now;

            // Incremento atómico del correlativo
            var correlativo = await documentSeriesRepository.IncrementAndGetCorrelativeAsync(
                request.IdEmpresa,
                request.IdSucursal,
                request.TipoDocumento,
                request.NumSerie,
                cancellationToken);

            if (correlativo <= 0)
                return Result.Failure<CreateSaleResponse>(SaleErrors.SerieNoEncontrada);

            var saleResult = Sale.Create(
                request.TipoDocumento,
                request.NumSerie,
                correlativo,
                request.IdCliente,
                request.NombreCliente,
                request.IdDocumentoIdentidad,
                request.NumDocumentoCliente,
                request.DireccionCliente,
                request.FlagIGV,
                request.FormaPago,
                request.TipoMoneda,
                request.TipoCambio,
                request.DescuentoGlobal,
                request.IdEmpresa,
                request.IdSucursal,
                request.IdEstacion,
                request.IdTurnoAsistencia,
                request.IdTrabajador,
                request.IdTrabajador2,
                request.IdSubdiario,
                request.Observaciones,
                request.IdUsuarioCreador,
                now);

            if (saleResult.IsFailure)
                return Result.Failure<CreateSaleResponse>(saleResult.Error);

            var sale = saleResult.Value;

            foreach (var item in request.Items)
            {
                var saleItem = SaleItem.Create(
                    idVenta: 0,   // EF lo asignará tras SaveChanges
                    item.IdArticulo,
                    item.Codigo,
                    item.Descripcion,
                    item.SiglaUnidad,
                    item.IdUnidad,
                    item.Cantidad,
                    item.PrecioUnitario,
                    item.TipoAfectacionIgv,
                    item.ValorVenta,
                    item.Descuento,
                    item.Isc,
                    item.Igv,
                    item.Icbper,
                    item.Subtotal,
                    item.IdClaseProducto,
                    item.IdTipoCliente);

                sale.AddItem(saleItem);
            }

            sale.SetTotals(
                request.ValorAfecto,
                request.ValorInafecto,
                request.ValorExonerado,
                request.ValorGratuito,
                request.TotalIsc,
                request.Igv,
                request.TotalIcbper,
                request.ImporteTotal);

            saleRepository.Add(sale);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var numeroFormateado = $"{request.NumSerie}-{correlativo:D8}";

            return Result.Success(new CreateSaleResponse(
                sale.IdVenta,
                sale.NumSerie,
                correlativo,
                numeroFormateado,
                sale.ImporteTotal));
        }
    }
}
