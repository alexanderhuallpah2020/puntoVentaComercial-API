using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.UpdateClient
{
    public sealed record UpdateClientCommand(
        int IdCliente,
        string Nombre,
        string CodValidadorDoc,
        string Direccion,
        string Telefono,
        int IdSucursal,
        int IdUsuarioModificador
    ) : ICommand;
}
