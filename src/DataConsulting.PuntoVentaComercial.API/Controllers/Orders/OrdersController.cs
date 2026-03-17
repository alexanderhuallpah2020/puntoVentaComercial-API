using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Orders.Commands.AnnulOrder;
using DataConsulting.PuntoVentaComercial.Application.Features.Orders.Commands.CreateOrder;
using DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetOrderById;
using DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetOrders;
using DataConsulting.PuntoVentaComercial.Application.Features.Orders.Queries.GetPrintableOrder;
using DataConsulting.PuntoVentaComercial.Application.Services.Print;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Orders
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ICommandHandler<CreateOrderCommand, CreateOrderResponse> _createHandler;
        private readonly ICommandHandler<AnnulOrderCommand> _annulHandler;
        private readonly IQueryHandler<GetOrdersQuery, GetOrdersResponse> _getOrdersHandler;
        private readonly IQueryHandler<GetOrderByIdQuery, GetOrderByIdResponse> _getByIdHandler;
        private readonly IQueryHandler<GetPrintableOrderQuery, GetPrintableOrderResponse> _printableHandler;
        private readonly IPrintService _printService;

        public OrdersController(
            ICommandHandler<CreateOrderCommand, CreateOrderResponse> createHandler,
            ICommandHandler<AnnulOrderCommand> annulHandler,
            IQueryHandler<GetOrdersQuery, GetOrdersResponse> getOrdersHandler,
            IQueryHandler<GetOrderByIdQuery, GetOrderByIdResponse> getByIdHandler,
            IQueryHandler<GetPrintableOrderQuery, GetPrintableOrderResponse> printableHandler,
            IPrintService printService)
        {
            _createHandler = createHandler;
            _annulHandler = annulHandler;
            _getOrdersHandler = getOrdersHandler;
            _getByIdHandler = getByIdHandler;
            _printableHandler = printableHandler;
            _printService = printService;
        }

        /// <summary>
        /// Registra un pedido. Incrementa el correlativo atómicamente.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new CreateOrderCommand(
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
                request.Observaciones,
                request.TipoMoneda,
                request.TipoCambio,
                request.DescuentoGlobal,
                request.Items.Select(i => new CreateOrderItemCommand(
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
                ? CreatedAtAction(nameof(GetById), new { id = result.Value.IdPedido }, result.Value)
                : BadRequest(result.Error);
        }

        /// <summary>
        /// Búsqueda de pedidos por empresa, sucursal, fechas, cliente, trabajador, estado.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int idEmpresa,
            [FromQuery] int idSucursal,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int? idCliente = null,
            [FromQuery] int? idTrabajador = null,
            [FromQuery] int? estado = null,
            [FromQuery] int pageSize = 100,
            CancellationToken cancellationToken = default)
        {
            var query = new GetOrdersQuery(
                idEmpresa, idSucursal,
                fechaDesde, fechaHasta,
                idCliente, idTrabajador,
                estado, pageSize);

            var result = await _getOrdersHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Detalle completo de un pedido (cabecera + ítems).
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var result = await _getByIdHandler.Handle(new GetOrderByIdQuery(id), cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Genera el PDF del ticket de un pedido (80 mm — impresora térmica).
        /// </summary>
        [HttpGet("{id:int}/print")]
        public async Task<IActionResult> Print(int id, CancellationToken cancellationToken = default)
        {
            var result = await _printableHandler.Handle(new GetPrintableOrderQuery(id), cancellationToken);
            if (result.IsFailure)
                return NotFound(result.Error);

            var pdf = _printService.GenerateOrderPdf(result.Value);
            return File(pdf, "application/pdf", $"pedido-{id}.pdf");
        }

        /// <summary>
        /// Anula un pedido. Estado pasa a 4 (EEstadoPedido.Anulado).
        /// </summary>
        [HttpPut("{id:int}/annul")]
        public async Task<IActionResult> Annul(
            int id,
            [FromQuery] int idUsuarioModificador,
            CancellationToken cancellationToken = default)
        {
            var result = await _annulHandler.Handle(
                new AnnulOrderCommand(id, idUsuarioModificador),
                cancellationToken);

            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }
    }
}
