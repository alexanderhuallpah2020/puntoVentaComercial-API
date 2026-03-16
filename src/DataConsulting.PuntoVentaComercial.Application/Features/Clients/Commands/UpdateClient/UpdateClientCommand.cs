using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.UpdateClient
{
    // NumDocumento e IdDocumentoIdentidad NO son editables (regla de negocio del legacy).
    public sealed record UpdateClientCommand(
        int IdCliente,
        string Nombre,
        string? NombreComercial,
        string Direccion,
        bool FlagIGV,
        decimal CreditoMaximo,
        short IdUsuarioModificador
    ) : ICommand;
}
