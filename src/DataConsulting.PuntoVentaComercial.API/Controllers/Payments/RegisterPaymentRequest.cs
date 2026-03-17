using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Payments
{
    public sealed record RegisterPaymentRequest(
        int IdVenta,
        int IdEmpresa,
        int IdSucursal,
        int IdCliente,
        decimal ImporteTotal,
        decimal ImportePagado,
        List<RegisterPaymentDetailRequest> Detalles,
        int IdUsuarioCreador);

    public sealed record RegisterPaymentDetailRequest(
        int IdFormaPago,
        string Descripcion,
        ETipoMoneda TipoMoneda,
        decimal TipoCambio,
        decimal Importe);
}
