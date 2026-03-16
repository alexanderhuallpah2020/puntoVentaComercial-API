namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    public interface IShiftRepository
    {
        Task<IReadOnlyList<Shift>> GetActiveShiftsAsync(
            int idEmpresa,
            TimeOnly horaActual,
            CancellationToken cancellationToken = default);
    }
}
