using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Payments.Commands.RegisterPayment;
using DataConsulting.PuntoVentaComercial.Application.Features.Payments.Queries.GetPaymentMethods;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Payments
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/payments")]
    //[Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly ICommandHandler<RegisterPaymentCommand, RegisterPaymentResponse> _registerHandler;
        private readonly IQueryHandler<GetPaymentMethodsQuery, GetPaymentMethodsResponse> _methodsHandler;

        public PaymentsController(
            ICommandHandler<RegisterPaymentCommand, RegisterPaymentResponse> registerHandler,
            IQueryHandler<GetPaymentMethodsQuery, GetPaymentMethodsResponse> methodsHandler)
        {
            _registerHandler = registerHandler;
            _methodsHandler = methodsHandler;
        }

        /// <summary>
        /// Retorna las formas de pago activas configuradas para la empresa.
        /// </summary>
        [HttpGet("methods")]
        public async Task<IActionResult> GetMethods(
            [FromQuery] int idEmpresa,
            CancellationToken cancellationToken = default)
        {
            var result = await _methodsHandler.Handle(
                new GetPaymentMethodsQuery(idEmpresa),
                cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Registra el cobro de una venta con una o más formas de pago.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Register(
            [FromBody] RegisterPaymentRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new RegisterPaymentCommand(
                request.IdVenta,
                request.IdEmpresa,
                request.IdSucursal,
                request.IdCliente,
                request.ImporteTotal,
                request.ImportePagado,
                request.Detalles.Select(d => new RegisterPaymentDetailCommand(
                    d.IdFormaPago,
                    d.Descripcion,
                    d.TipoMoneda,
                    d.TipoCambio,
                    d.Importe)).ToList(),
                request.IdUsuarioCreador);

            var result = await _registerHandler.Handle(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Obtiene el cobro registrado para una venta.
        /// </summary>
        [HttpGet("{idVenta:int}")]
        public async Task<IActionResult> GetByVenta(int idVenta, CancellationToken cancellationToken = default)
        {
            // La consulta de pago se resuelve vía el handler de RegisterPayment que ya cargó la venta.
            // Para un GET puro, se podría crear GetPaymentByVentaQuery; aquí devolvemos 501 como placeholder
            // hasta que se implemente la query dedicada.
            return StatusCode(StatusCodes.Status501NotImplemented,
                new { Message = "Consulta de pago pendiente de implementar en la siguiente iteración." });
        }
    }
}
