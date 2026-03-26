using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Ventas;
using static DataConsulting.PuntoVentaComercial.Domain.Ventas.CodigosSunat;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.AnularVenta;

internal sealed class AnularVentaCommandHandler(
    IVentaRepository ventaRepository,
    ICurrentUserService currentUserService)
    : ICommandHandler<AnularVentaCommand, bool>
{
    public async Task<Result<bool>> Handle(
        AnularVentaCommand request, CancellationToken cancellationToken)
    {
        // 1. Cargar venta
        var venta = await ventaRepository.GetByIdAsync(request.IdVenta, cancellationToken);
        if (venta is null)
            return Result.Failure<bool>(VentaErrors.NotFound(request.IdVenta));

        // 2. Estado
        if (venta.Estado == EstadoVenta.Anulado)
            return Result.Failure<bool>(VentaErrors.YaAnulada);

        if (venta.Estado != EstadoVenta.Activo)
            return Result.Failure<bool>(VentaErrors.EstadoInvalidoParaAnular);

        // 3. No se puede anular si ya fue aceptada por SUNAT
        if (venta.CodigoSunat == CodigosSunat.Aceptado)
            return Result.Failure<bool>(VentaErrors.AceptadaEnSunat);

        // 4. Flags de la BD (GuiaRemision, NotaCD, ValorCambio, EstadoContable)
        var guards = await ventaRepository.GetAnulacionGuardsAsync(request.IdVenta, cancellationToken);
        if (guards is not null)
        {
            if (guards.TieneGuiaRemision)  return Result.Failure<bool>(VentaErrors.TieneGuiaRemision);
            if (guards.TieneNotaCD)        return Result.Failure<bool>(VentaErrors.TieneNotaCD);
            if (guards.TieneValorCambio)   return Result.Failure<bool>(VentaErrors.TieneValorCambio);
            if (guards.EstaContabilizada)  return Result.Failure<bool>(VentaErrors.EstaContabilizada);
        }

        // 5. Sin movimientos de ingreso al almacén referenciando esta venta
        if (await ventaRepository.TieneMovimientoIngresoAsync(request.IdVenta, cancellationToken))
            return Result.Failure<bool>(VentaErrors.TieneIngresoAlmacen);

        // 6. Período contable abierto
        if (!await ventaRepository.PeriodoAbiertoPorFechaAsync(venta.IdSucursal, venta.FechaEmision, cancellationToken))
            return Result.Failure<bool>(VentaErrors.PeriodoCerrado);

        // 7. Ejecutar anulación completa vía SPs
        await ventaRepository.AnularVentaCompletaAsync(
            request.IdVenta, venta.IdTipoVenta, currentUserService.UserName, cancellationToken);

        return Result.Success(true);
    }
}
