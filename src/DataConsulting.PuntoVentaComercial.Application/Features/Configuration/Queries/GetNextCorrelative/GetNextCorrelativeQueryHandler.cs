using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Configuration;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetNextCorrelative
{
    internal sealed class GetNextCorrelativeQueryHandler(IDocumentSeriesRepository repository)
        : IQueryHandler<GetNextCorrelativeQuery, GetNextCorrelativeResponse>
    {
        public async Task<Result<GetNextCorrelativeResponse>> Handle(
            GetNextCorrelativeQuery query,
            CancellationToken cancellationToken)
        {
            var correlativo = await repository.GetNextCorrelativeAsync(
                query.IdEmpresa,
                query.IdSucursal,
                query.TipoDocumento,
                query.NumSerie,
                cancellationToken);

            if (correlativo == 0)
                return Result.Failure<GetNextCorrelativeResponse>(ConfigurationErrors.SerieNoEncontrada);

            var numeroFormateado = $"{query.NumSerie}-{correlativo:D6}";

            return Result.Success(new GetNextCorrelativeResponse(
                (int)query.TipoDocumento,
                query.NumSerie,
                correlativo,
                numeroFormateado
            ));
        }
    }
}
