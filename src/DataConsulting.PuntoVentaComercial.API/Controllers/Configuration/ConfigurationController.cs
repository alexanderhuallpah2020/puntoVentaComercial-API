using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetConstants;
using DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetCurrencies;
using DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetExchangeRates;
using DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetSellers;
using DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetShifts;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Configuration
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    //[Authorize]
    public class ConfigurationController : ControllerBase
    {
        private readonly IQueryHandler<GetExchangeRatesQuery, GetExchangeRatesResponse> _exchangeRatesHandler;
        private readonly IQueryHandler<GetCurrenciesQuery, GetCurrenciesResponse> _currenciesHandler;
        private readonly IQueryHandler<GetShiftsQuery, GetShiftsResponse> _shiftsHandler;
        private readonly IQueryHandler<GetSellersQuery, GetSellersResponse> _sellersHandler;
        private readonly IQueryHandler<GetConstantsQuery, GetConstantsResponse> _constantsHandler;

        public ConfigurationController(
            IQueryHandler<GetExchangeRatesQuery, GetExchangeRatesResponse> exchangeRatesHandler,
            IQueryHandler<GetCurrenciesQuery, GetCurrenciesResponse> currenciesHandler,
            IQueryHandler<GetShiftsQuery, GetShiftsResponse> shiftsHandler,
            IQueryHandler<GetSellersQuery, GetSellersResponse> sellersHandler,
            IQueryHandler<GetConstantsQuery, GetConstantsResponse> constantsHandler)
        {
            _exchangeRatesHandler = exchangeRatesHandler;
            _currenciesHandler = currenciesHandler;
            _shiftsHandler = shiftsHandler;
            _sellersHandler = sellersHandler;
            _constantsHandler = constantsHandler;
        }

        /// <summary>
        /// Tipos de cambio por empresa y fecha. Si no se indica fecha, usa la fecha actual.
        /// </summary>
        [HttpGet("api/v{version:apiVersion}/exchange-rates")]
        public async Task<IActionResult> GetExchangeRates(
            [FromQuery] int idEmpresa,
            [FromQuery] string? fecha = null,
            CancellationToken cancellationToken = default)
        {
            DateOnly? fechaParsed = fecha is not null && DateOnly.TryParse(fecha, out var f) ? f : null;
            var query = new GetExchangeRatesQuery(idEmpresa, fechaParsed);
            var result = await _exchangeRatesHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Catálogo de monedas activas.
        /// </summary>
        [HttpGet("api/v{version:apiVersion}/currencies")]
        public async Task<IActionResult> GetCurrencies(CancellationToken cancellationToken = default)
        {
            var result = await _currenciesHandler.Handle(new GetCurrenciesQuery(), cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Turnos activos para la empresa. Filtra por hora actual si se indica.
        /// </summary>
        [HttpGet("api/v{version:apiVersion}/shifts")]
        public async Task<IActionResult> GetShifts(
            [FromQuery] int idEmpresa,
            [FromQuery] string? hora = null,
            CancellationToken cancellationToken = default)
        {
            TimeOnly? horaParsed = hora is not null && TimeOnly.TryParse(hora, out var h) ? h : null;
            var query = new GetShiftsQuery(idEmpresa, horaParsed);
            var result = await _shiftsHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Vendedores activos por empresa.
        /// </summary>
        [HttpGet("api/v{version:apiVersion}/sellers")]
        public async Task<IActionResult> GetSellers(
            [FromQuery] int idEmpresa,
            CancellationToken cancellationToken = default)
        {
            var result = await _sellersHandler.Handle(new GetSellersQuery(idEmpresa), cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Constantes del sistema por empresa y sucursal (montos máximos, flags, etc.).
        /// </summary>
        [HttpGet("api/v{version:apiVersion}/constants")]
        public async Task<IActionResult> GetConstants(
            [FromQuery] int idEmpresa,
            [FromQuery] int idSucursal,
            CancellationToken cancellationToken = default)
        {
            var result = await _constantsHandler.Handle(new GetConstantsQuery(idEmpresa, idSucursal), cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
    }
}
