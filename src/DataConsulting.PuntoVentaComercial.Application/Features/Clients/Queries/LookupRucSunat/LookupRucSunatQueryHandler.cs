using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Services.Sunat;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.LookupRucSunat
{
    internal sealed class LookupRucSunatQueryHandler(
        ISunatClientLookupService sunatService)
        : IQueryHandler<LookupRucSunatQuery, RucSunatResponse>
    {
        public async Task<Result<RucSunatResponse>> Handle(
            LookupRucSunatQuery request,
            CancellationToken cancellationToken)
        {
            if (!RucValidator.Validate(request.Ruc))
            {
                return Result.Failure<RucSunatResponse>(ClientErrors.RucInvalido);
            }

            RucInfo? info = await sunatService.LookupByRucAsync(request.Ruc, cancellationToken);

            if (info is null)
            {
                return Result.Failure<RucSunatResponse>(ClientErrors.NotFound(0));
            }

            var response = new RucSunatResponse(
                request.Ruc,
                info.RazonSocial,
                info.Direccion,
                info.Estado,
                info.TipoContribuyente);

            return Result.Success(response);
        }
    }
}
