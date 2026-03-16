namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetShifts
{
    public sealed record GetShiftsResponse(IReadOnlyList<ShiftDto> Shifts);

    public sealed record ShiftDto(
        int Id,
        string Descripcion,
        string HoraInicio,
        string HoraFin
    );
}
