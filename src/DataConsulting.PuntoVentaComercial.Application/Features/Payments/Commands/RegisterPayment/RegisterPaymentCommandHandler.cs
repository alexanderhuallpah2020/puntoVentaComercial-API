using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Payments;
using DataConsulting.PuntoVentaComercial.Domain.Sales;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Payments.Commands.RegisterPayment
{
    internal sealed class RegisterPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        ISaleRepository saleRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<RegisterPaymentCommand, RegisterPaymentResponse>
    {
        public async Task<Result<RegisterPaymentResponse>> Handle(
            RegisterPaymentCommand request,
            CancellationToken cancellationToken)
        {
            var sale = await saleRepository.GetByIdAsync(request.IdVenta, cancellationToken);
            if (sale is null)
                return Result.Failure<RegisterPaymentResponse>(PaymentErrors.VentaNoEncontrada);

            if (sale.Estado == 0)
                return Result.Failure<RegisterPaymentResponse>(SaleErrors.Anulada(request.IdVenta));

            var now = DateTime.Now;

            var payment = Payment.Create(
                request.IdVenta,
                request.IdEmpresa,
                request.IdSucursal,
                request.IdCliente,
                request.ImporteTotal,
                request.ImportePagado,
                request.IdUsuarioCreador,
                now);

            foreach (var d in request.Detalles)
            {
                var detalle = PaymentDetail.Create(
                    idOperacion: 0,   // EF asignará tras SaveChanges
                    d.IdFormaPago,
                    d.Descripcion,
                    d.TipoMoneda,
                    d.TipoCambio,
                    d.Importe);

                payment.AddDetalle(detalle);
            }

            paymentRepository.Add(payment);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new RegisterPaymentResponse(
                payment.IdOperacion,
                payment.IdVenta,
                payment.ImporteTotal,
                payment.ImportePagado,
                payment.Vuelto,
                payment.Credito));
        }
    }
}
