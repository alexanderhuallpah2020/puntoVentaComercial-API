namespace DataConsulting.PuntoVentaComercial.Domain.OperacionesPago;

public interface IOperacionPagoRepository
{
    void Add(OperacionPago operacionPago);
    // MAX(NroOperacion) + 1 filtrado por IdEmpresa y TipoOperacion
    Task<int> GetNextNroOperacionAsync(short idEmpresa, byte tipoOperacion, CancellationToken ct);
}
