using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.AnnulSale
{
    public sealed record AnnulSaleCommand(
        int IdVenta,
        int IdUsuarioModificador
    ) : ICommand;
}
