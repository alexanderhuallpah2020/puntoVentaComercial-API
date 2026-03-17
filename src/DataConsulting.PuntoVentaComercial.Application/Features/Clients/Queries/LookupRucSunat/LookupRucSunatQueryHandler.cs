using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Services.Sunat;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.LookupRucSunat
{
    internal sealed class LookupRucSunatQueryHandler(ISunatClientLookupService sunatService)
        : IQueryHandler<LookupRucSunatQuery, LookupRucSunatResponse>
    {
        public async Task<Result<LookupRucSunatResponse>> Handle(
            LookupRucSunatQuery query,
            CancellationToken cancellationToken)
        {
            var info = await sunatService.LookupByRucAsync(query.Ruc, cancellationToken);

            if (info is null)
                return Result.Failure<LookupRucSunatResponse>(ClientErrors.RucNoEncontradoEnSunat);

            return Result.Success(new LookupRucSunatResponse(
                info.Ruc,
                info.RazonSocial,
                info.Direccion,
                info.Ubigeo,
                info.Departamento,
                info.Provincia,
                info.Distrito,
                info.Activo));
        }
    }
}
