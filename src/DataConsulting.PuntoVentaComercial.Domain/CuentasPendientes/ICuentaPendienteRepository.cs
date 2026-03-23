namespace DataConsulting.PuntoVentaComercial.Domain.CuentasPendientes;

public interface ICuentaPendienteRepository
{
    void Add(CuentaPendiente cuentaPendiente);
    Task<CuentaPendiente?> GetByVentaAsync(
        short idEmpresa, int idVenta, short secuencia, CancellationToken ct);
}
