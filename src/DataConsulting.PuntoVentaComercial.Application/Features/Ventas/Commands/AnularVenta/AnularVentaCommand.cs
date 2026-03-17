using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.AnularVenta;

public sealed record AnularVentaCommand(
    int IdVenta,
    string? MotivoAnulacion) : ICommand<bool>;
