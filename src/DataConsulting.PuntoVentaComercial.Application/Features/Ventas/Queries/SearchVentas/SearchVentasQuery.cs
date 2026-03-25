using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Queries.SearchVentas;

public sealed record SearchVentasQuery(
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    string? NombreCliente,
    string? NumSerieA,
    int? NumDocumento,
    short? IdTipoDocumento,
    string? Estado,
    int Page,
    int PageSize) : IQuery<SearchVentasResponse>;
