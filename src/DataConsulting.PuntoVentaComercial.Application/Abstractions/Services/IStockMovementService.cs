using DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.CreateVenta;

namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

public interface IStockMovementService
{
    Task DescuentoStockVentaAsync(
        short idEmpresa,
        short idSucursal,
        int idVenta,
        int idCliente,
        short idVendedor,
        short idTipoMoneda,
        decimal importeTotal,
        IList<CreateVentaDetalleDto> detalles,
        CancellationToken cancellationToken);
}
