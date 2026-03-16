using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Configuration;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetConstants
{
    internal sealed class GetConstantsQueryHandler(ISystemConstantRepository repository)
        : IQueryHandler<GetConstantsQuery, GetConstantsResponse>
    {
        public async Task<Result<GetConstantsResponse>> Handle(
            GetConstantsQuery query,
            CancellationToken cancellationToken)
        {
            var constants = await repository.GetConstantsAsync(
                query.IdEmpresa,
                query.IdSucursal,
                cancellationToken);

            var dtos = constants.Select(c => new SystemConstantDto(
                c.Clave,
                c.Valor,
                c.Descripcion
            )).ToList();

            return Result.Success(new GetConstantsResponse(dtos));
        }
    }
}
