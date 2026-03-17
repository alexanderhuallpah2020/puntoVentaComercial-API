using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Services;

internal sealed class SunatClientLookupStub : ISunatClientLookupService
{
    public Task<Result<SunatClienteInfo>> LookupAsync(string ruc, CancellationToken ct) =>
        Task.FromResult(Result.Failure<SunatClienteInfo>(ClienteErrors.SunatNoDisponible));
}
