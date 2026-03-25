using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.EnviarVentaSunat;

public sealed record EnviarVentaSunatCommand(int IdVenta) : ICommand<EnviarVentaSunatResponse>;

public sealed record EnviarVentaSunatResponse(
    string CodigoRespuesta,
    string Descripcion);
