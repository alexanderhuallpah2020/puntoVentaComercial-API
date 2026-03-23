using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Domain.CuentasPendientes;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using DataConsulting.PuntoVentaComercial.Domain.OperacionesPago;
using DataConsulting.PuntoVentaComercial.Domain.Ventas;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.CreateVenta;

internal sealed class CreateVentaCommandHandler(
    IVentaRepository ventaRepository,
    IClienteRepository clienteRepository,
    ICuentaPendienteRepository cuentaPendienteRepository,
    IOperacionPagoRepository operacionPagoRepository,
    IUnitOfWork unitOfWork,
    IStockMovementService stockMovementService,
    IPoliticService politicService,
    ICurrentUserService currentUserService,
    IDateTimeService dateTimeService)
    : ICommandHandler<CreateVentaCommand, int>
{
    public async Task<Result<int>> Handle(
        CreateVentaCommand request, CancellationToken cancellationToken)
    {
        string usuario = currentUserService.UserName;
        DateTime ahora = dateTimeService.Now;

        // 0 no es un IdTurnoAsistencia válido — se trata igual que null
        short? idTurnoAsistencia = request.IdTurnoAsistencia is null or 0
            ? null
            : request.IdTurnoAsistencia;

        var cliente = await clienteRepository.GetByIdAsync(request.IdCliente, cancellationToken);
        if (cliente is null)
            return Result.Failure<int>(VentaErrors.ClienteNoEncontrado(request.IdCliente));

        int nextNumero = await ventaRepository.GetNextNumeroDocumentoAsync(
            request.IdSucursal, request.IdTipoDocumento, request.NumSerieA, cancellationToken);

        // NroCorrelativo contable: solo cuando hay subdiario asignado
        int nroCorrelativo = 0;
        if (request.IdSubdiario is > 0)
        {
            int maxCorrelativo = await ventaRepository.GetNroCorrelativoVentaAsync(
                DateTime.Now, request.IdSubdiario.Value, cancellationToken);
            nroCorrelativo = maxCorrelativo + 1;
        }

        // NroOperacion para OperacionPago — MAX+1 antes de abrir transacción
        int nroOperacion = await operacionPagoRepository.GetNextNroOperacionAsync(
            request.IdEmpresa, tipoOperacion: 2, cancellationToken);

        short correlativo = 1;
        var detalles = request.Detalles
            .Select(d => VentaDetalle.Create(
                correlativo++,
                request.IdEmpresa,
                d.IdArticulo,
                d.IdUnidad,
                d.DescripcionArticulo,
                d.Cantidad,
                d.PrecioUnitario,
                d.ImporteDescuento,
                d.TipoDescuento,
                d.FlagExonerado,
                d.FlagRegalo,
                d.IdTipoAfectoIGV,
                d.Isc,
                d.ValorICBPER,
                ahora))
            .ToList();

        var pagos = request.Pagos
            .Select(p => VentaPago.Create(p.IdFormaPago, p.IdTipoMoneda, p.Importe, ahora))
            .ToList();

        short cuotaCorrelativo = 1;
        var cuotas = request.Cuotas
            .Select(c => VentaCuota.Create(
                cuotaCorrelativo++,
                c.FechaCuota,
                c.Monto,
                $"Cuota{cuotaCorrelativo:D3}"))
            .ToList();

        var result = Venta.Create(
            request.IdEmpresa,
            request.IdSucursal,
            request.IdEstacionTrabajo,
            request.IdSubSede,
            request.IdTipoDocumento,
            request.NumSerie,
            request.NumSerieA,
            nextNumero,
            nroCorrelativo,
            request.IdCliente,
            request.IdTipoCliente,
            request.IdVendedor,
            request.IdVendedor2,
            idTurnoAsistencia,
            request.IdTipoMoneda,
            request.TipoCambio,
            request.ValorNeto,
            request.ImporteDescuento,
            request.ImporteDescuentoGlobal,
            request.PorcentajeDescuentoGlobal,
            request.ValorVenta,
            request.Igv,
            request.ValorExonerado,
            request.Isc,
            request.ValorICBPER,
            request.ImporteTotal,
            request.ImportePagado,
            request.ImporteVuelto,
            request.RedondeoTotal,
            request.IdFormaPago,
            request.FlagDescPorcentaje,
            request.IdSubdiario,
            detalles,
            pagos,
            cuotas,
            usuarioCreador: usuario,
            ahora,
            request.ClienteNombre,
            request.ClienteDireccion,
            request.ClienteDocumento,
            request.Observacion,
            request.PuntosBonus,
            request.Referencias,
            request.ClienteCodValidadorDoc);

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        var venta = result.Value;

        // Lectura pura antes de abrir la transacción — PoliticService usa ADO.NET directo
        bool habilitarSinStock = await politicService.HasPoliticAsync(
            "admin", EPolitica.HabilitarVentaSinStock, cancellationToken);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // 1. INSERT Venta + VentaEmision + VentaDetalle + VentaPago + VentaCuota
            ventaRepository.Add(venta);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            int idVenta = venta.Id;
            int idEntidad = cliente.IdEntidadRef ?? 0;

            // 2. INSERT CuentaPendiente (saldo inicial = ImporteTotal)
            var cuentaPendiente = CuentaPendiente.Create(
                idEmpresa:      request.IdEmpresa,
                tipoOperacion:  2,
                idOperacion:    idVenta,
                secuencia:      1,
                idTipoMoneda:   request.IdTipoMoneda,
                importe:        request.ImporteTotal,
                fechaPago:      venta.FechaEmision,
                idEntidad:      idEntidad,
                idTipoDocumento: request.IdTipoDocumento,
                flagTipo:       1,
                glosa:          "",
                usuarioCreador: usuario,
                ahora);

            cuentaPendienteRepository.Add(cuentaPendiente);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // 3. INSERT OperacionPago + OperacionPagoDetalle + CuentaAmortizacion
            var pagoDetalle = OperacionPagoDetalle.Create(
                idEmpresa:     request.IdEmpresa,
                tipoOperacion: 2,
                nroOperacion:  nroOperacion,
                secuencia:     1,
                idFormaPago:   request.IdFormaPago,
                idTipoMoneda:  request.IdTipoMoneda,
                importe:       request.ImporteTotal);

            var amortizacion = CuentaAmortizacion.Create(
                idEmpresa:        request.IdEmpresa,
                tipoOperacion:    2,
                nroOperacion:     nroOperacion,
                tipoOperacionRef: 2,
                idOperacion:      idVenta,
                secuencia:        1,
                importe:          request.ImporteTotal);

            var operacionPago = OperacionPago.Create(
                idEmpresa:      request.IdEmpresa,
                tipoOperacion:  2,
                nroOperacion:   nroOperacion,
                fechaEmision:   venta.FechaEmision,
                idTipoMoneda:   request.IdTipoMoneda,
                importeTotal:   request.ImporteTotal,
                idSucursal:     request.IdSucursal,
                idTrabajador:   request.IdVendedor,
                idEstacion:     request.IdEstacionTrabajo,
                tipoCambio:     request.TipoCambio,
                idEntidad:      idEntidad,
                observaciones:  "",
                idTurnoAsistencia: request.IdTurnoAsistencia == 0 ? null : request.IdTurnoAsistencia,
                estadoContable: 1,
                usuarioCreador: usuario,
                ahora,
                detalles:       [pagoDetalle],
                amortizaciones: [amortizacion]);

            operacionPagoRepository.Add(operacionPago);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // 4. UPDATE CuentaPendiente — marcar como liquidado (Saldo=0, FlagLiquidado=1)
            cuentaPendiente.MarcarLiquidado(usuario);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Descuento de stock (solo si política no habilita venta sin stock)
            if (!habilitarSinStock)
            {
                await stockMovementService.DescuentoStockVentaAsync(
                    request.IdEmpresa,
                    request.IdSucursal,
                    idVenta,
                    request.IdCliente,
                    request.IdVendedor,
                    request.IdTipoMoneda,
                    request.ImporteTotal,
                    request.Detalles,
                    cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return Result.Success(idVenta);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
