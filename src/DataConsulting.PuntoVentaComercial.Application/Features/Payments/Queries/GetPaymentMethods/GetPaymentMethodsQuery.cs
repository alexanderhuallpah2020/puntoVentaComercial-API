using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Payments.Queries.GetPaymentMethods
{
    public sealed record GetPaymentMethodsQuery(int IdEmpresa) : IQuery<GetPaymentMethodsResponse>;
}
