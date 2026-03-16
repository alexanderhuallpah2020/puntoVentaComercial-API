using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Configuration;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetShifts
{
    internal sealed class GetShiftsQueryHandler(IShiftRepository repository)
        : IQueryHandler<GetShiftsQuery, GetShiftsResponse>
    {
        public async Task<Result<GetShiftsResponse>> Handle(
            GetShiftsQuery query,
            CancellationToken cancellationToken)
        {
            var hora = query.HoraActual ?? TimeOnly.FromDateTime(DateTime.Now);

            var shifts = await repository.GetActiveShiftsAsync(
                query.IdEmpresa, hora, cancellationToken);

            if (shifts.Count == 0)
                return Result.Failure<GetShiftsResponse>(ConfigurationErrors.SinTurnos);

            var dtos = shifts.Select(s => new ShiftDto(
                s.Id,
                s.Descripcion,
                s.HoraInicio.ToString("HH:mm"),
                s.HoraFin.ToString("HH:mm")
            )).ToList();

            return Result.Success(new GetShiftsResponse(dtos));
        }
    }
}
