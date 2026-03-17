using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Orders.Commands.AnnulOrder
{
    public sealed record AnnulOrderCommand(
        int IdPedido,
        int IdUsuarioModificador
    ) : ICommand;
}
