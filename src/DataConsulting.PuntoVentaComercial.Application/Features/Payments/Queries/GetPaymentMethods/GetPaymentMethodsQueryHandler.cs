using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Payments;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Payments.Queries.GetPaymentMethods
{
    internal sealed class GetPaymentMethodsQueryHandler(IPaymentRepository paymentRepository)
        : IQueryHandler<GetPaymentMethodsQuery, GetPaymentMethodsResponse>
    {
        public async Task<Result<GetPaymentMethodsResponse>> Handle(
            GetPaymentMethodsQuery query,
            CancellationToken cancellationToken)
        {
            var methods = await paymentRepository.GetPaymentMethodsAsync(
                query.IdEmpresa,
                cancellationToken);

            var dtos = methods.Select(m => new PaymentMethodDto(
                m.IdFormaPago,
                m.Descripcion,
                m.Activo)).ToList();

            return Result.Success(new GetPaymentMethodsResponse(dtos));
        }
    }
}
