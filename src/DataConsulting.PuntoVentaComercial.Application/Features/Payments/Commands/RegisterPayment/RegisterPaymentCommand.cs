using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Payments.Commands.RegisterPayment
{
    public sealed record RegisterPaymentCommand(
        int IdVenta,
        int IdEmpresa,
        int IdSucursal,
        int IdCliente,
        decimal ImporteTotal,
        decimal ImportePagado,
        List<RegisterPaymentDetailCommand> Detalles,
        int IdUsuarioCreador
    ) : ICommand<RegisterPaymentResponse>;

    public sealed record RegisterPaymentDetailCommand(
        int IdFormaPago,
        string Descripcion,
        ETipoMoneda TipoMoneda,
        decimal TipoCambio,
        decimal Importe);
}
