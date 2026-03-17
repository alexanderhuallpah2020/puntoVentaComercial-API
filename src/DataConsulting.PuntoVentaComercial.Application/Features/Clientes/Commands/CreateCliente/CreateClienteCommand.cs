using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Commands.CreateCliente;

public sealed record CreateClienteCommand(
    string Nombre,
    int? IdDocumentoIdentidad,
    string? NumDocumento,
    string? CodValidadorDoc,
    short IdPais,
    string DireccionLocal,
    string? Telefono1,
    short IdSucursal,
    string? NombreComercial = null) : ICommand<int>;
