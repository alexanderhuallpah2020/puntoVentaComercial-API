using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Cash.Commands.AnnulVaultDeposit;
using DataConsulting.PuntoVentaComercial.Application.Features.Cash.Commands.CreateVaultDeposit;
using DataConsulting.PuntoVentaComercial.Application.Features.Cash.Queries.GetAvailableCash;
using DataConsulting.PuntoVentaComercial.Application.Features.Cash.Queries.GetVaultDeposits;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Cash
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/vault")]
    [Authorize]
    public class CashController : ControllerBase
    {
        private readonly ICommandHandler<CreateVaultDepositCommand, CreateVaultDepositResponse> _createHandler;
        private readonly ICommandHandler<AnnulVaultDepositCommand> _annulHandler;
        private readonly IQueryHandler<GetVaultDepositsQuery, GetVaultDepositsResponse> _getDepositsHandler;
        private readonly IQueryHandler<GetAvailableCashQuery, GetAvailableCashResponse> _getAvailableCashHandler;

        public CashController(
            ICommandHandler<CreateVaultDepositCommand, CreateVaultDepositResponse> createHandler,
            ICommandHandler<AnnulVaultDepositCommand> annulHandler,
            IQueryHandler<GetVaultDepositsQuery, GetVaultDepositsResponse> getDepositsHandler,
            IQueryHandler<GetAvailableCashQuery, GetAvailableCashResponse> getAvailableCashHandler)
        {
            _createHandler = createHandler;
            _annulHandler = annulHandler;
            _getDepositsHandler = getDepositsHandler;
            _getAvailableCashHandler = getAvailableCashHandler;
        }

        /// <summary>
        /// Efectivo disponible por trabajador/isla/moneda/fecha (recaudado menos depositado).
        /// </summary>
        [HttpGet("available-cash")]
        public async Task<IActionResult> GetAvailableCash(
            [FromQuery] int idEmpresa,
            [FromQuery] int idSucursal,
            [FromQuery] int? idTrabajador = null,
            [FromQuery] int? idIsla = null,
            [FromQuery] ETipoMoneda tipoMoneda = ETipoMoneda.Soles,
            [FromQuery] DateTime? fecha = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetAvailableCashQuery(
                idEmpresa, idSucursal,
                idTrabajador, idIsla,
                tipoMoneda,
                fecha ?? DateTime.Today);

            var result = await _getAvailableCashHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Registra un depósito en bóveda.
        /// </summary>
        [HttpPost("deposits")]
        public async Task<IActionResult> CreateDeposit(
            [FromBody] CreateVaultDepositRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new CreateVaultDepositCommand(
                request.IdEmpresa,
                request.IdSucursal,
                request.IdTrabajador,
                request.IdIsla,
                request.IdTurnoAsistencia,
                request.TipoDocumento,
                request.NumSerie,
                request.NumDocumento,
                request.TipoMoneda,
                request.TipoCambio,
                request.Importe,
                request.IdFormaPago,
                request.Glosa,
                request.IdUsuarioCreador);

            var result = await _createHandler.Handle(command, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Búsqueda de depósitos por isla, documento, serie, número, moneda, fecha.
        /// </summary>
        [HttpGet("deposits")]
        public async Task<IActionResult> GetDeposits(
            [FromQuery] int idEmpresa,
            [FromQuery] int idSucursal,
            [FromQuery] int? idIsla = null,
            [FromQuery] int? idTrabajador = null,
            [FromQuery] EDocumento? tipoDocumento = null,
            [FromQuery] string? numSerie = null,
            [FromQuery] string? numDocumento = null,
            [FromQuery] ETipoMoneda? tipoMoneda = null,
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int pageSize = 100,
            CancellationToken cancellationToken = default)
        {
            var query = new GetVaultDepositsQuery(
                idEmpresa, idSucursal,
                idIsla, idTrabajador,
                tipoDocumento, numSerie, numDocumento,
                tipoMoneda, fechaDesde, fechaHasta,
                pageSize);

            var result = await _getDepositsHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Anula un depósito en bóveda.
        /// </summary>
        [HttpPut("deposits/{id:int}/annul")]
        public async Task<IActionResult> AnnulDeposit(
            int id,
            [FromQuery] int idUsuarioModificador,
            CancellationToken cancellationToken = default)
        {
            var result = await _annulHandler.Handle(
                new AnnulVaultDepositCommand(id, idUsuarioModificador),
                cancellationToken);

            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }
    }
}
