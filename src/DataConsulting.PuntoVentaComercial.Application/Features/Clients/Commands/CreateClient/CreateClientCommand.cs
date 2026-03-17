using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.CreateClient
{
    public sealed record CreateClientCommand(
        string Nombre,
        EDocumentoIdentidad IdDocumentoIdentidad,
        string NumDocumento,
        string CodValidadorDoc,
        int IdPais,
        int IdSucursal,
        string Direccion,
        string Telefono,
        int IdUsuarioCreador
    ) : ICommand<CreateClientResponse>;
}
