using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.AnnulSale;
using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CalculateSale;
using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CreateSale;
using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetSaleById;
using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetSales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Sales
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/sales")]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly ICommandHandler<CalculateSaleCommand, CalculateSaleResponse> _calculateHandler;
        private readonly ICommandHandler<CreateSaleCommand, CreateSaleResponse> _createHandler;
        private readonly ICommandHandler<AnnulSaleCommand> _annulHandler;
        private readonly IQueryHandler<GetSalesQuery, GetSalesResponse> _getSalesHandler;
        private readonly IQueryHandler<GetSaleByIdQuery, GetSaleByIdResponse> _getByIdHandler;

        public SalesController(
            ICommandHandler<CalculateSaleCommand, CalculateSaleResponse> calculateHandler,
            ICommandHandler<CreateSaleCommand, CreateSaleResponse> createHandler,
            ICommandHandler<AnnulSaleCommand> annulHandler,
            IQueryHandler<GetSalesQuery, GetSalesResponse> getSalesHandler,
            IQueryHandler<GetSaleByIdQuery, GetSaleByIdResponse> getByIdHandler)
        {
            _calculateHandler = calculateHandler;
            _createHandler = createHandler;
            _annulHandler = annulHandler;
            _getSalesHandler = getSalesHandler;
            _getByIdHandler = getByIdHandler;
        }

        /// <summary>
        /// Calcula totales de una venta (IGV, ISC, ICBPER, descuentos) sin persistir datos.
        /// Determina automáticamente el tipo de documento (Factura/Boleta) según el documento del cliente.
        /// </summary>
        [HttpPost("calculate")]
        [AllowAnonymous]
        public async Task<IActionResult> Calculate(
            [FromBody] CalculateSaleRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new CalculateSaleCommand(
                TipoDocumentoCliente: request.TipoDocumentoCliente,
                Items: request.Items.Select(i => new CalculateSaleItemCommand(
                    ProductoId: i.ProductoId,
                    Cantidad: i.Cantidad,
                    PrecioUnitario: i.PrecioUnitario,
                    TipoAfectacionIgv: i.TipoAfectacionIgv,
                    TipoIsc: i.TipoIsc,
                    TasaIsc: i.TasaIsc,
                    MontoFijoIsc: i.MontoFijoIsc,
                    EsBolsaPlastica: i.EsBolsaPlastica)).ToList(),
                DescuentoGlobal: request.DescuentoGlobal,
                FormaPago: request.FormaPago);

            var result = await _calculateHandler.Handle(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Registra una venta completa. Incrementa el correlativo atómicamente.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateSaleRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new CreateSaleCommand(
                request.TipoDocumento,
                request.NumSerie,
                request.IdCliente,
                request.NombreCliente,
                request.IdDocumentoIdentidad,
                request.NumDocumentoCliente,
                request.DireccionCliente,
                request.FlagIGV,
                request.IdEmpresa,
                request.IdSucursal,
                request.IdEstacion,
                request.IdTurnoAsistencia,
                request.IdTrabajador,
                request.IdTrabajador2,
                request.IdSubdiario,
                request.Observaciones,
                request.FormaPago,
                request.TipoMoneda,
                request.TipoCambio,
                request.DescuentoGlobal,
                request.Items.Select(i => new CreateSaleItemCommand(
                    i.IdArticulo, i.Codigo, i.Descripcion, i.SiglaUnidad, i.IdUnidad,
                    i.Cantidad, i.PrecioUnitario, i.TipoAfectacionIgv,
                    i.ValorVenta, i.Descuento, i.Isc, i.Igv, i.Icbper, i.Subtotal,
                    i.IdClaseProducto, i.IdTipoCliente)).ToList(),
                request.ValorAfecto,
                request.ValorInafecto,
                request.ValorExonerado,
                request.ValorGratuito,
                request.TotalIsc,
                request.Igv,
                request.TotalIcbper,
                request.ImporteTotal,
                request.IdUsuarioCreador);

            var result = await _createHandler.Handle(command, cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Value.IdVenta }, result.Value)
                : BadRequest(result.Error);
        }

        /// <summary>
        /// Búsqueda de ventas por empresa, sucursal, fechas, tipo documento, cliente, estado.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int idEmpresa,
            [FromQuery] int idSucursal,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int? idTipoDocumento = null,
            [FromQuery] string? numSerie = null,
            [FromQuery] long? correlativo = null,
            [FromQuery] int? idCliente = null,
            [FromQuery] int? estado = null,
            [FromQuery] int pageSize = 100,
            CancellationToken cancellationToken = default)
        {
            var query = new GetSalesQuery(
                idEmpresa, idSucursal,
                fechaDesde, fechaHasta,
                idTipoDocumento, numSerie, correlativo,
                idCliente, estado, pageSize);

            var result = await _getSalesHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Detalle completo de una venta (cabecera + ítems).
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var result = await _getByIdHandler.Handle(new GetSaleByIdQuery(id), cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Anula una venta. Estado pasa a 0.
        /// </summary>
        [HttpPut("{id:int}/annul")]
        public async Task<IActionResult> Annul(
            int id,
            [FromQuery] int idUsuarioModificador,
            CancellationToken cancellationToken = default)
        {
            var result = await _annulHandler.Handle(
                new AnnulSaleCommand(id, idUsuarioModificador),
                cancellationToken);

            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }
    }
}
