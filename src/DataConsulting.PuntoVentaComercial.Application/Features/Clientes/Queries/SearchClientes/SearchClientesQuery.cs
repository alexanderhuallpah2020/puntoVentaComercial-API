using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.SearchClientes;

public sealed record SearchClientesQuery(
    string? Nombre,
    string? NumDocumento,
    short? IdPais,
    int? IdDocIdentidad,
    int Page = 1,
    int PageSize = 20) : IQuery<SearchClientesResponse>;
