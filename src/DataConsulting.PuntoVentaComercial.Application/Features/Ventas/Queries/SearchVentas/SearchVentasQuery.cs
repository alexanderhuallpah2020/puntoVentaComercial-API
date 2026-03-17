using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Queries.SearchVentas;

public sealed record SearchVentasQuery(
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    int? IdCliente,
    short? NumSerie,
    short? IdTipoDocumento,
    string? Estado,
    short? IdSucursal,
    int Page,
    int PageSize) : IQuery<SearchVentasResponse>;
