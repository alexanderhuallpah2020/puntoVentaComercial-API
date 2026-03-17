using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Orders;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Commands.AnnulOrder
{
    internal sealed class AnnulOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<AnnulOrderCommand>
    {
        public async Task<Result> Handle(AnnulOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(request.IdPedido, cancellationToken);
            if (order is null)
                return Result.Failure(OrderErrors.NotFound(request.IdPedido));

            var annulResult = order.Annul(request.IdUsuarioModificador, DateTime.Now);
            if (annulResult.IsFailure)
                return annulResult;

            orderRepository.Update(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
