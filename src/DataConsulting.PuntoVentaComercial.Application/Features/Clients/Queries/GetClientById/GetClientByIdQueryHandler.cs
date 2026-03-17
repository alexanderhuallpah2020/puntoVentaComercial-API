using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.GetClientById
{
    internal sealed class GetClientByIdQueryHandler(IClientRepository clientRepository)
        : IQueryHandler<GetClientByIdQuery, GetClientByIdResponse>
    {
        public async Task<Result<GetClientByIdResponse>> Handle(
            GetClientByIdQuery query,
            CancellationToken cancellationToken)
        {
            var client = await clientRepository.GetByIdAsync(query.IdCliente, cancellationToken);
            if (client is null)
                return Result.Failure<GetClientByIdResponse>(ClientErrors.ClienteNoEncontrado);

            return Result.Success(new GetClientByIdResponse(
                client.IdCliente,
                client.Nombre,
                client.NombreComercial,
                (int)client.IdDocumentoIdentidad,
                client.NumDocumento,
                client.CodValidadorDoc,
                client.IdPais,
                client.IdTipoCliente,
                client.FlagIGV,
                client.CreditoMaximo,
                client.EstadoCliente));
        }
    }
}
