using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Commands.CreateCliente;
using DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Commands.UpdateCliente;
using DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.GetClienteAddresses;
using DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.GetClienteById;
using DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.LookupRucSunat;
using DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.SearchClientes;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Clientes;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/clientes")]
public sealed class ClientesController(
    ICommandHandler<CreateClienteCommand, int> createHandler,
    ICommandHandler<UpdateClienteCommand, bool> updateHandler,
    IQueryHandler<GetClienteByIdQuery, GetClienteByIdResponse> getByIdHandler,
    IQueryHandler<SearchClientesQuery, SearchClientesResponse> searchHandler,
    IQueryHandler<GetClienteAddressesQuery, IList<ClienteLocalResponse>> addressesHandler,
    IQueryHandler<LookupRucSunatQuery, LookupRucSunatResponse> sunatHandler)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? nombre,
        [FromQuery] string? numDocumento,
        [FromQuery] short? idPais,
        [FromQuery] int? idDocIdentidad,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await searchHandler.Handle(
            new SearchClientesQuery(nombre, numDocumento, idPais, idDocIdentidad, page, pageSize), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateClienteRequest request,
        CancellationToken ct)
    {
        var command = new CreateClienteCommand(
            request.Nombre, request.IdDocumentoIdentidad, request.NumDocumento,
            request.CodValidadorDoc, request.IdPais, request.DireccionLocal,
            request.Telefono1, request.IdSucursal, request.NombreComercial);

        var result = await createHandler.Handle(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await getByIdHandler.Handle(new GetClienteByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateClienteRequest request,
        CancellationToken ct)
    {
        var command = new UpdateClienteCommand(
            id, request.Nombre, request.IdDocumentoIdentidad, request.NumDocumento,
            request.CodValidadorDoc, request.IdPais, request.DireccionLocal, request.Telefono1);

        var result = await updateHandler.Handle(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:int}/addresses")]
    public async Task<IActionResult> GetAddresses(int id, CancellationToken ct)
    {
        var result = await addressesHandler.Handle(new GetClienteAddressesQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("sunat/{ruc}")]
    public async Task<IActionResult> LookupSunat(string ruc, CancellationToken ct)
    {
        var result = await sunatHandler.Handle(new LookupRucSunatQuery(ruc), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
