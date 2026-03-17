using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

public sealed record SunatClienteInfo(
    string Ruc,
    string RazonSocial,
    string Direccion,
    string Estado,
    string Condicion);

public interface ISunatClientLookupService
{
    Task<Result<SunatClienteInfo>> LookupAsync(string ruc, CancellationToken ct);
}
