using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Queries.GetClienteById;

internal sealed class GetClienteByIdQueryHandler(IClienteRepository repository)
    : IQueryHandler<GetClienteByIdQuery, GetClienteByIdResponse>
{
    public async Task<Result<GetClienteByIdResponse>> Handle(
        GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await repository.GetByIdAsync(request.IdCliente, cancellationToken);
        if (cliente is null)
            return Result.Failure<GetClienteByIdResponse>(ClienteErrors.NotFound(request.IdCliente));

        return Result.Success(MapToResponse(cliente));
    }

    private static GetClienteByIdResponse MapToResponse(Cliente c) => new(
        c.Id,
        c.Nombre,
        c.NombreComercial,
        c.IdDocumentoIdentidad,
        c.NumDocumento,
        c.CodValidadorDoc,
        c.IdPais,
        c.EstadoCliente,
        c.IdTrabajadorRef is null or 0,
        c.ClienteLocales.Select(l => new ClienteLocalResponse(
            l.Id, l.IdSucursal, l.DireccionLocal, l.Telefono1, l.Estado)).ToList());
}
