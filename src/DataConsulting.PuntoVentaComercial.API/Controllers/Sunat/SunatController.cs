using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Commands.SubmitSaleToSunat;
using DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Queries.GetPendingSubmissions;
using DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Queries.GetSubmissionStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Sunat
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/sunat")]
    [Authorize]
    public class SunatController : ControllerBase
    {
        private readonly ICommandHandler<SubmitSaleToSunatCommand, SubmitSaleToSunatResponse> _submitHandler;
        private readonly IQueryHandler<GetPendingSubmissionsQuery, GetPendingSubmissionsResponse> _pendingHandler;
        private readonly IQueryHandler<GetSubmissionStatusQuery, GetSubmissionStatusResponse> _statusHandler;

        public SunatController(
            ICommandHandler<SubmitSaleToSunatCommand, SubmitSaleToSunatResponse> submitHandler,
            IQueryHandler<GetPendingSubmissionsQuery, GetPendingSubmissionsResponse> pendingHandler,
            IQueryHandler<GetSubmissionStatusQuery, GetSubmissionStatusResponse> statusHandler)
        {
            _submitHandler = submitHandler;
            _pendingHandler = pendingHandler;
            _statusHandler = statusHandler;
        }

        /// <summary>
        /// Comprobantes electrónicos pendientes de envío o rechazados por SUNAT.
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending(
            [FromQuery] int idEmpresa,
            [FromQuery] int idSucursal,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int pageSize = 100,
            CancellationToken cancellationToken = default)
        {
            var query = new GetPendingSubmissionsQuery(
                idEmpresa, idSucursal,
                fechaDesde, fechaHasta,
                pageSize);

            var result = await _pendingHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Envía (o reenvía) un comprobante electrónico a SUNAT.
        /// Idempotente: si ya fue aceptado retorna 409 Conflict.
        /// </summary>
        [HttpPost("submissions/{idVenta:int}")]
        public async Task<IActionResult> Submit(
            int idVenta,
            CancellationToken cancellationToken = default)
        {
            var result = await _submitHandler.Handle(
                new SubmitSaleToSunatCommand(idVenta),
                cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Estado de envío SUNAT de una venta (NoEnviado / Enviado / Aceptado / Rechazado).
        /// </summary>
        [HttpGet("submissions/{idVenta:int}/status")]
        public async Task<IActionResult> GetStatus(
            int idVenta,
            CancellationToken cancellationToken = default)
        {
            var result = await _statusHandler.Handle(
                new GetSubmissionStatusQuery(idVenta),
                cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
    }
}
