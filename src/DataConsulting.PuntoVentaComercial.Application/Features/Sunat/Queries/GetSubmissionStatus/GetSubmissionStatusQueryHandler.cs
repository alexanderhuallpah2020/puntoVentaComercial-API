using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Sunat;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Queries.GetSubmissionStatus
{
    internal sealed class GetSubmissionStatusQueryHandler(ISunatSubmissionRepository repository)
        : IQueryHandler<GetSubmissionStatusQuery, GetSubmissionStatusResponse>
    {
        public async Task<Result<GetSubmissionStatusResponse>> Handle(
            GetSubmissionStatusQuery query,
            CancellationToken cancellationToken)
        {
            var submission = await repository.GetByIdVentaAsync(query.IdVenta, cancellationToken);

            if (submission is null)
                return Result.Failure<GetSubmissionStatusResponse>(SunatErrors.NotFound(query.IdVenta));

            return Result.Success(new GetSubmissionStatusResponse(
                submission.IdVenta,
                submission.Estado,
                submission.CodigoRespuesta,
                submission.MensajeRespuesta,
                submission.XmlHash,
                submission.NumTicket,
                submission.Intentos,
                submission.FechaEnvio,
                submission.FechaModificacion));
        }
    }
}
