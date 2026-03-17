using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Configuration;
using DataConsulting.PuntoVentaComercial.Domain.Orders;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Commands.CreateOrder
{
    internal sealed class CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IDocumentSeriesRepository documentSeriesRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<CreateOrderCommand, CreateOrderResponse>
    {
        public async Task<Result<CreateOrderResponse>> Handle(
            CreateOrderCommand request,
            CancellationToken cancellationToken)
        {
            var now = DateTime.Now;

            var correlativo = await documentSeriesRepository.IncrementAndGetCorrelativeAsync(
                request.IdEmpresa,
                request.IdSucursal,
                request.TipoDocumento,
                request.NumSerie,
                cancellationToken);

            if (correlativo <= 0)
                return Result.Failure<CreateOrderResponse>(OrderErrors.SerieNoEncontrada);

            var orderResult = Order.Create(
                request.TipoDocumento,
                request.NumSerie,
                correlativo,
                request.IdCliente,
                request.NombreCliente,
                request.IdDocumentoIdentidad,
                request.NumDocumentoCliente,
                request.DireccionCliente,
                request.FlagIGV,
                request.TipoMoneda,
                request.TipoCambio,
                request.DescuentoGlobal,
                request.IdEmpresa,
                request.IdSucursal,
                request.IdEstacion,
                request.IdTurnoAsistencia,
                request.IdTrabajador,
                request.IdTrabajador2,
                request.Observaciones,
                request.IdUsuarioCreador,
                now);

            if (orderResult.IsFailure)
                return Result.Failure<CreateOrderResponse>(orderResult.Error);

            var order = orderResult.Value;

            foreach (var item in request.Items)
            {
                order.AddItem(OrderItem.Create(
                    idPedido: 0,
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
                    item.IdTipoCliente));
            }

            order.SetTotals(
                request.ValorAfecto,
                request.ValorInafecto,
                request.ValorExonerado,
                request.ValorGratuito,
                request.TotalIsc,
                request.Igv,
                request.TotalIcbper,
                request.ImporteTotal);

            orderRepository.Add(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new CreateOrderResponse(
                order.IdPedido,
                order.NumSerie,
                correlativo,
                $"{request.NumSerie}-{correlativo:D8}",
                order.ImporteTotal));
        }
    }
}
