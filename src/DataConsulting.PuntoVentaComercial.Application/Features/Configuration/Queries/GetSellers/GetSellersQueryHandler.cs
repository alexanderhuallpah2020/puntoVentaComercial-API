using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Configuration;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetSellers
{
    internal sealed class GetSellersQueryHandler(ISellerRepository repository)
        : IQueryHandler<GetSellersQuery, GetSellersResponse>
    {
        public async Task<Result<GetSellersResponse>> Handle(
            GetSellersQuery query,
            CancellationToken cancellationToken)
        {
            var sellers = await repository.GetActiveSellersAsync(
                query.IdEmpresa, cancellationToken);

            var dtos = sellers.Select(s => new SellerDto(
                s.Id,
                s.Nombres,
                s.Apellidos,
                s.NombreCompleto,
                s.Codigo,
                s.PorcentajeDescuentoMaximo
            )).ToList();

            return Result.Success(new GetSellersResponse(dtos));
        }
    }
}
