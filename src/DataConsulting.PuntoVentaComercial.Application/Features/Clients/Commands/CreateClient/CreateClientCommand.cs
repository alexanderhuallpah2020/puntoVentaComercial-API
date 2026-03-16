using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.CreateClient
{
    public sealed record CreateClientCommand(
        string Nombre,
        string? NombreComercial,
        EDocumentoIdentidad IdDocumentoIdentidad,
        string NumDocumento,
        string? CodValidadorDoc,
        int IdPais,
        string Direccion,
        string Telefono1,
        bool FlagIGV,
        decimal CreditoMaximo,
        int IdEmpresa,
        int IdSucursal,
        short IdUsuarioCreador
    ) : ICommand<int>;
}
