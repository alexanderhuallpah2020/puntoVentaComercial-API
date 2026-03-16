using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Commands.CreateSegmentoSunat;
using DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetAllSegmentosSunat;
using DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase;
using DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase.V1;
using DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase.V2;
using DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoSunatById;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.SegmentosSunat
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/segmentos-sunat")]
    public class SegmentoSunatController : ControllerBase
    {
        private readonly IQueryHandler<GetAllSegmentosSunatQuery, List<GetAllSegmentosSunatResponse>> _getAllHandler;
        private readonly IQueryHandler<GetSegmentoSunatByIdQuery, GetSegmentoSunatByIdResponse> _getByIdHandler;
        private readonly IQueryHandler<GetSegmentoFamiliaClaseV1Query, List<GetSegmentoFamiliaClaseResponse>> _familiasClasesV1Handler;
        private readonly IQueryHandler<GetSegmentoFamiliaClaseV2Query, List<GetSegmentoFamiliaClaseResponse>> _familiasClasesV2Handler;
        private readonly ICommandHandler<CreateSegmentoSunatCommand, int> _createHandler;

        public SegmentoSunatController(IQueryHandler<GetAllSegmentosSunatQuery, List<GetAllSegmentosSunatResponse>> getAllHandler, IQueryHandler<GetSegmentoSunatByIdQuery, GetSegmentoSunatByIdResponse> getByIdHandler, IQueryHandler<GetSegmentoFamiliaClaseV1Query, List<GetSegmentoFamiliaClaseResponse>> familiasClasesV1Handler, IQueryHandler<GetSegmentoFamiliaClaseV2Query, List<GetSegmentoFamiliaClaseResponse>> familiasClasesV2Handler, ICommandHandler<CreateSegmentoSunatCommand, int> createHandler)
        {
            _getAllHandler = getAllHandler;
            _getByIdHandler = getByIdHandler;
            _familiasClasesV1Handler = familiasClasesV1Handler;
            _familiasClasesV2Handler = familiasClasesV2Handler;
            _createHandler = createHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var query = new GetAllSegmentosSunatQuery();
            var result = await _getAllHandler.Handle(query, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var query = new GetSegmentoSunatByIdQuery(id);
            var result = await _getByIdHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpGet("familias-clases-linq")]
        public async Task<IActionResult> GetFamiliasClasesLinq(CancellationToken cancellationToken = default)
        {
            var query = new GetSegmentoFamiliaClaseV1Query();
            var result = await _familiasClasesV1Handler.Handle(query, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("familias-clases-dapper")]
        public async Task<IActionResult> GetFamiliasClasesDapper(CancellationToken cancellationToken = default)
        {
            var query = new GetSegmentoFamiliaClaseV2Query();
            var result = await _familiasClasesV2Handler.Handle(query, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
           [FromBody] CreateSegmentoSunatRequest request,
           CancellationToken cancellationToken = default)
        {
            var command = new CreateSegmentoSunatCommand(
                request.Codigo,
                request.Descripcion);

            Result<int> result = await _createHandler.Handle(command, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
    }
}
