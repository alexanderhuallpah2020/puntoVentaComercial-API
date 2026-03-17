using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Queries.GetVentaById;

public sealed record GetVentaByIdQuery(int IdVenta) : IQuery<GetVentaByIdResponse>;
