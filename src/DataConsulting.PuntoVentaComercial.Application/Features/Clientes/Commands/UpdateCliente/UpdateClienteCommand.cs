using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Commands.UpdateCliente;

public sealed record UpdateClienteCommand(
    int IdCliente,
    string Nombre,
    int? IdDocumentoIdentidad,
    string? NumDocumento,
    string? CodValidadorDoc,
    short IdPais,
    string DireccionLocal,
    string? Telefono1) : ICommand;
