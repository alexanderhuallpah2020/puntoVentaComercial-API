using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.LookupRucSunat;

internal sealed class LookupRucSunatQueryHandler(ISunatClientLookupService sunatService)
    : IQueryHandler<LookupRucSunatQuery, LookupRucSunatResponse>
{
    public async Task<Result<LookupRucSunatResponse>> Handle(
        LookupRucSunatQuery request, CancellationToken cancellationToken)
    {
        if (!RucValidator.EsValido(request.Ruc))
            return Result.Failure<LookupRucSunatResponse>(ClienteErrors.RucInvalido(request.Ruc));

        var result = await sunatService.LookupAsync(request.Ruc, cancellationToken);
        if (result.IsFailure)
            return Result.Failure<LookupRucSunatResponse>(result.Error);

        var info = result.Value;
        return Result.Success(new LookupRucSunatResponse(
            info.Ruc, info.RazonSocial, info.Direccion, info.Estado, info.Condicion));
    }
}
