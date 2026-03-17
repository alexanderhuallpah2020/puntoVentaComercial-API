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
        private readonly ICommandHandler<CreateClientCommand, CreateClientResponse> _createHandler;
        private readonly ICommandHandler<UpdateClientCommand> _updateHandler;
        private readonly IQueryHandler<SearchClientsQuery, SearchClientsResponse> _searchHandler;
        private readonly IQueryHandler<GetClientByIdQuery, GetClientByIdResponse> _getByIdHandler;
        private readonly IQueryHandler<GetClientAddressesQuery, GetClientAddressesResponse> _addressesHandler;
        private readonly IQueryHandler<LookupRucSunatQuery, LookupRucSunatResponse> _sunatHandler;

        public ClientsController(
            ICommandHandler<CreateClientCommand, CreateClientResponse> createHandler,
            ICommandHandler<UpdateClientCommand> updateHandler,
            IQueryHandler<SearchClientsQuery, SearchClientsResponse> searchHandler,
            IQueryHandler<GetClientByIdQuery, GetClientByIdResponse> getByIdHandler,
            IQueryHandler<GetClientAddressesQuery, GetClientAddressesResponse> addressesHandler,
            IQueryHandler<LookupRucSunatQuery, LookupRucSunatResponse> sunatHandler)
        {
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _searchHandler = searchHandler;
            _getByIdHandler = getByIdHandler;
            _addressesHandler = addressesHandler;
            _sunatHandler = sunatHandler;
        }

        /// <summary>
        /// Busca clientes por nombre, número de documento o tipo de documento.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string? nombre = null,
            [FromQuery] string? numDocumento = null,
            [FromQuery] int? idDocumentoIdentidad = null,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var query = new SearchClientsQuery(nombre, numDocumento, idDocumentoIdentidad, pageSize);
            var result = await _searchHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>
        /// Crea un nuevo cliente con su dirección inicial.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateClientRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new CreateClientCommand(
                request.Nombre,
                request.IdDocumentoIdentidad,
                request.NumDocumento,
                request.CodValidadorDoc,
                request.IdPais,
                request.IdSucursal,
                request.Direccion,
                request.Telefono,
                request.IdUsuarioCreador);

            var result = await _createHandler.Handle(command, cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Value.IdCliente }, result.Value)
                : BadRequest(result.Error);
        }

        /// <summary>
        /// Obtiene el detalle de un cliente por ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
        {
            var query = new GetClientByIdQuery(id);
            var result = await _getByIdHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Actualiza nombre, código validador y dirección de un cliente.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateClientRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateClientCommand(
                id,
                request.Nombre,
                request.CodValidadorDoc,
                request.Direccion,
                request.Telefono,
                request.IdSucursal,
                request.IdUsuarioModificador);

            var result = await _updateHandler.Handle(command, cancellationToken);
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        /// <summary>
        /// Lista las direcciones (locales) registradas de un cliente.
        /// </summary>
        [HttpGet("{id:int}/addresses")]
        public async Task<IActionResult> GetAddresses(int id, CancellationToken cancellationToken = default)
        {
            var query = new GetClientAddressesQuery(id);
            var result = await _addressesHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Consulta datos de un contribuyente en SUNAT por RUC.
        /// </summary>
        [HttpGet("sunat/{ruc}")]
        public async Task<IActionResult> LookupSunat(string ruc, CancellationToken cancellationToken = default)
        {
            var query = new LookupRucSunatQuery(ruc);
            var result = await _sunatHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
    }
}
