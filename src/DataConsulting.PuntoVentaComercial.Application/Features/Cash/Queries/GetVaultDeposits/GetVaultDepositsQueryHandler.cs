using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Cash;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Queries.GetVaultDeposits
{
    internal sealed class GetVaultDepositsQueryHandler(IVaultDepositRepository repository)
        : IQueryHandler<GetVaultDepositsQuery, GetVaultDepositsResponse>
    {
        public async Task<Result<GetVaultDepositsResponse>> Handle(
            GetVaultDepositsQuery query,
            CancellationToken cancellationToken)
        {
            var items = await repository.SearchAsync(
                query.IdEmpresa,
                query.IdSucursal,
                query.IdIsla,
                query.IdTrabajador,
                query.TipoDocumento,
                query.NumSerie,
                query.NumDocumento,
                query.TipoMoneda,
                query.FechaDesde,
                query.FechaHasta,
                query.PageSize,
                cancellationToken);

            return Result.Success(new GetVaultDepositsResponse(items));
        }
    }
}
