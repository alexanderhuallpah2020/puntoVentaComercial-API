using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Configuration;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetDocumentSeries
{
    internal sealed class GetDocumentSeriesQueryHandler(IDocumentSeriesRepository repository)
        : IQueryHandler<GetDocumentSeriesQuery, GetDocumentSeriesResponse>
    {
        public async Task<Result<GetDocumentSeriesResponse>> Handle(
            GetDocumentSeriesQuery query,
            CancellationToken cancellationToken)
        {
            var series = await repository.GetByEstacionAsync(
                query.IdEmpresa,
                query.IdSucursal,
                query.IdEstacion,
                cancellationToken);

            if (series.Count == 0)
                return Result.Failure<GetDocumentSeriesResponse>(ConfigurationErrors.SerieNoEncontrada);

            var dtos = series.Select(s => new DocumentSeriesDto(
                s.IdEmpresa,
                s.IdSucursal,
                s.IdEstacion,
                (int)s.TipoDocumento,
                s.TipoDocumento.ToString(),
                s.NumSerie,
                s.UltimoCorrelativo
            )).ToList();

            return Result.Success(new GetDocumentSeriesResponse(dtos));
        }
    }
}
