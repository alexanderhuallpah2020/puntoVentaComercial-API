using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Cash;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Queries.GetAvailableCash
{
    internal sealed class GetAvailableCashQueryHandler(IVaultDepositRepository repository)
        : IQueryHandler<GetAvailableCashQuery, GetAvailableCashResponse>
    {
        public async Task<Result<GetAvailableCashResponse>> Handle(
            GetAvailableCashQuery query,
            CancellationToken cancellationToken)
        {
            var disponible = await repository.GetAvailableCashAsync(
                query.IdEmpresa,
                query.IdSucursal,
                query.IdTrabajador,
                query.IdIsla,
                query.TipoMoneda,
                query.Fecha,
                cancellationToken);

            return Result.Success(new GetAvailableCashResponse(
                disponible,
                query.TipoMoneda,
                query.Fecha));
        }
    }
}
