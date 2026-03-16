using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.CreateClient;
using DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.UpdateClient;
using DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.GetClientAddresses;
using DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.GetClientById;
using DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.LookupRucSunat;
using DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.SearchClients;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Clients
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/clients")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IQueryHandler<SearchClientsQuery, List<ClientSummaryResponse>> _searchHandler;
        private readonly ICommandHandler<CreateClientCommand, int> _createHandler;
        private readonly IQueryHandler<GetClientByIdQuery, ClientDetailResponse> _getByIdHandler;
        private readonly ICommandHandler<UpdateClientCommand> _updateHandler;
        private readonly IQueryHandler<GetClientAddressesQuery, List<ClientAddressResponse>> _addressesHandler;
        private readonly IQueryHandler<LookupRucSunatQuery, RucSunatResponse> _lookupRucHandler;

        public ClientsController(
            IQueryHandler<SearchClientsQuery, List<ClientSummaryResponse>> searchHandler,
            ICommandHandler<CreateClientCommand, int> createHandler,
            IQueryHandler<GetClientByIdQuery, ClientDetailResponse> getByIdHandler,
            ICommandHandler<UpdateClientCommand> updateHandler,
            IQueryHandler<GetClientAddressesQuery, List<ClientAddressResponse>> addressesHandler,
            IQueryHandler<LookupRucSunatQuery, RucSunatResponse> lookupRucHandler)
        {
            _searchHandler = searchHandler;
            _createHandler = createHandler;
            _getByIdHandler = getByIdHandler;
            _updateHandler = updateHandler;
            _addressesHandler = addressesHandler;
            _lookupRucHandler = lookupRucHandler;
        }

        /// <summary>
        /// Busca clientes por nombre, número de documento y/o tipo de documento.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string? nombre,
            [FromQuery] string? numDocumento,
            [FromQuery] EDocumentoIdentidad? tipoDocumento,
            [FromQuery] int idEmpresa,
            CancellationToken cancellationToken = default)
        {
            var query = new SearchClientsQuery(nombre, numDocumento, tipoDocumento, idEmpresa);
            Result<List<ClientSummaryResponse>> result = await _searchHandler.Handle(query, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Crea un nuevo cliente con validación de DNI o RUC peruano.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateClientRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Extraer IdUsuarioCreador, IdEmpresa e IdSucursal del JWT claim en lugar de hardcodear.
            short idUsuarioCreador = 1;
            int idSucursal = 1;

            var command = new CreateClientCommand(
                request.Nombre,
                request.NombreComercial,
                request.IdDocumentoIdentidad,
                request.NumDocumento,
                request.CodValidadorDoc,
                request.IdPais,
                request.Direccion,
                request.Telefono1,
                request.FlagIGV,
                request.CreditoMaximo,
                request.IdEmpresa,
                idSucursal,
                idUsuarioCreador);

            Result<int> result = await _createHandler.Handle(command, cancellationToken);

            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.Conflict => Conflict(result.Error),
                    _ => BadRequest(result.Error)
                };
            }

            return Ok(new { idCliente = result.Value });
        }

        /// <summary>
        /// Obtiene el detalle completo de un cliente por su Id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var query = new GetClientByIdQuery(id);
            Result<ClientDetailResponse> result = await _getByIdHandler.Handle(query, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(result.Error);
        }

        /// <summary>
        /// Actualiza los datos editables de un cliente (nombre, dirección, crédito).
        /// El número de documento no es modificable.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateClientRequest request,
            CancellationToken cancellationToken = default)
        {
            // TODO: Extraer IdUsuarioModificador del JWT claim.
            short idUsuarioModificador = 1;

            var command = new UpdateClientCommand(
                id,
                request.Nombre,
                request.NombreComercial,
                request.Direccion,
                request.FlagIGV,
                request.CreditoMaximo,
                idUsuarioModificador);

            Result result = await _updateHandler.Handle(command, cancellationToken);

            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.NotFound => NotFound(result.Error),
                    _ => BadRequest(result.Error)
                };
            }

            return NoContent();
        }

        /// <summary>
        /// Obtiene las direcciones/locales registradas para un cliente.
        /// Se usa para el combo de dirección en el POS al facturar.
        /// </summary>
        [HttpGet("{id:int}/addresses")]
        public async Task<IActionResult> GetAddresses(
            int id,
            CancellationToken cancellationToken = default)
        {
            var query = new GetClientAddressesQuery(id);
            Result<List<ClientAddressResponse>> result = await _addressesHandler.Handle(query, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Consulta los datos de un contribuyente en SUNAT por su número de RUC.
        /// Requiere integración activa con el servicio SUNAT (Fase B9).
        /// </summary>
        [HttpGet("sunat/{ruc}")]
        public async Task<IActionResult> LookupRucSunat(
            string ruc,
            CancellationToken cancellationToken = default)
        {
            var query = new LookupRucSunatQuery(ruc);
            Result<RucSunatResponse> result = await _lookupRucHandler.Handle(query, cancellationToken);

            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.NotFound => NotFound(result.Error),
                    ErrorType.Failure => BadRequest(result.Error),
                    _ => BadRequest(result.Error)
                };
            }

            return Ok(result.Value);
        }
    }

    // ─── Request DTOs ────────────────────────────────────────────────────────────

    public sealed record CreateClientRequest(
        string Nombre,
        string? NombreComercial,
        EDocumentoIdentidad IdDocumentoIdentidad,
        string NumDocumento,
        string? CodValidadorDoc,
        int IdPais,
        string Direccion,
        string Telefono1,
        bool FlagIGV,
        decimal CreditoMaximo,
        int IdEmpresa);

    public sealed record UpdateClientRequest(
        string Nombre,
        string? NombreComercial,
        string Direccion,
        bool FlagIGV,
        decimal CreditoMaximo);
}
