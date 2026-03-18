using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.AnularVenta;
using DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.CreateVenta;
using DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Queries.GetVentaById;
using DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Queries.SearchVentas;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Ventas;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/ventas")]
public sealed class VentasController(
    ICommandHandler<CreateVentaCommand, int> createHandler,
    ICommandHandler<AnularVentaCommand, bool> anularHandler,
    IQueryHandler<GetVentaByIdQuery, GetVentaByIdResponse> getByIdHandler,
    IQueryHandler<SearchVentasQuery, SearchVentasResponse> searchHandler)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVentaRequest request,
        CancellationToken ct)
    {
        var command = new CreateVentaCommand(
            request.IdEmpresa,
            request.IdSucursal,
            request.IdEstacionTrabajo,
            request.IdSubSede,
            request.IdTipoDocumento,
            request.NumSerie,
            request.NumSerieA,
            request.IdCliente,
            request.IdTipoCliente,
            request.IdVendedor,
            request.IdVendedor2,
            request.IdTurnoAsistencia,
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
            request.Detalles.Select(d => new CreateVentaDetalleDto(
                d.IdArticulo, d.IdUnidad, d.DescripcionArticulo, d.Cantidad,
                d.PrecioUnitario, d.ImporteDescuento, d.TipoDescuento, d.FlagExonerado,
                d.FlagRegalo, d.IdTipoAfectoIGV, d.Isc, d.ValorICBPER)).ToList(),
            request.Pagos.Select(p => new CreateVentaPagoDto(
                p.IdFormaPago, p.IdTipoMoneda, p.Importe)).ToList());

        var result = await createHandler.Handle(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await getByIdHandler.Handle(new GetVentaByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] int? idCliente,
        [FromQuery] short? numSerie,
        [FromQuery] short? idTipoDocumento,
        [FromQuery] string? estado,
        [FromQuery] short? idSucursal,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await searchHandler.Handle(
            new SearchVentasQuery(
                fechaDesde, fechaHasta, idCliente, numSerie,
                idTipoDocumento, estado, idSucursal, page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id:int}/anular")]
    public async Task<IActionResult> Anular(
        int id,
        [FromBody] AnularVentaRequest request,
        CancellationToken ct)
    {
        var result = await anularHandler.Handle(
            new AnularVentaCommand(id, request.MotivoAnulacion), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
