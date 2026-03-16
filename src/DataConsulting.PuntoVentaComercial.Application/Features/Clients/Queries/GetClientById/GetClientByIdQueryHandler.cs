using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.GetClientById
{
    internal sealed class GetClientByIdQueryHandler(
        IClientRepository clientRepository)
        : IQueryHandler<GetClientByIdQuery, ClientDetailResponse>
    {
        public async Task<Result<ClientDetailResponse>> Handle(
            GetClientByIdQuery request,
            CancellationToken cancellationToken)
        {
            Client? client = await clientRepository.GetByIdAsync(request.IdCliente, cancellationToken);

            if (client is null)
            {
                return Result.Failure<ClientDetailResponse>(ClientErrors.NotFound(request.IdCliente));
            }

            var response = new ClientDetailResponse(
                client.IdCliente,
                client.Nombre,
                client.NombreComercial,
                client.IdDocumentoIdentidad,
                client.IdDocumentoIdentidad.ToString(),
                client.NumDocumento,
                client.CodValidadorDoc,
                client.IdPais,
                client.Direccion,
                client.FlagIGV,
                client.CreditoMaximo,
                client.EstadoCliente,
                client.FechaAlta,
                client.FechaBaja);

            return Result.Success(response);
        }
    }
}
