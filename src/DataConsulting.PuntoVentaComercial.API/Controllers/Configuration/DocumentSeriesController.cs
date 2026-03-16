using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetDocumentSeries;
using DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetNextCorrelative;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Configuration
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/document-series")]
    [Authorize]
    public class DocumentSeriesController : ControllerBase
    {
        private readonly IQueryHandler<GetDocumentSeriesQuery, GetDocumentSeriesResponse> _seriesHandler;
        private readonly IQueryHandler<GetNextCorrelativeQuery, GetNextCorrelativeResponse> _correlativeHandler;

        public DocumentSeriesController(
            IQueryHandler<GetDocumentSeriesQuery, GetDocumentSeriesResponse> seriesHandler,
            IQueryHandler<GetNextCorrelativeQuery, GetNextCorrelativeResponse> correlativeHandler)
        {
            _seriesHandler = seriesHandler;
            _correlativeHandler = correlativeHandler;
        }

        /// <summary>
        /// Series de documentos asignadas a la estación/sucursal/empresa.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDocumentSeries(
            [FromQuery] int idEmpresa,
            [FromQuery] int idSucursal,
            [FromQuery] int idEstacion,
            CancellationToken cancellationToken = default)
        {
            var query = new GetDocumentSeriesQuery(idEmpresa, idSucursal, idEstacion);
            var result = await _seriesHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Próximo correlativo disponible para un tipo de documento y serie.
        /// </summary>
        [HttpGet("next-correlative")]
        public async Task<IActionResult> GetNextCorrelative(
            [FromQuery] int idEmpresa,
            [FromQuery] int idSucursal,
            [FromQuery] EDocumento tipoDocumento,
            [FromQuery] string numSerie,
            CancellationToken cancellationToken = default)
        {
            var query = new GetNextCorrelativeQuery(idEmpresa, idSucursal, tipoDocumento, numSerie);
            var result = await _correlativeHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
    }
}
