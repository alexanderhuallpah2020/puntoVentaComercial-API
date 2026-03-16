using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.UpdateClient
{
    internal sealed class UpdateClientCommandHandler(
        IClientRepository clientRepository)
        : ICommandHandler<UpdateClientCommand>
    {
        public async Task<Result> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            Client? client = await clientRepository.GetByIdAsync(request.IdCliente, cancellationToken);

            if (client is null)
            {
                return Result.Failure(ClientErrors.NotFound(request.IdCliente));
            }

            Result updateResult = client.Update(
                request.Nombre,
                request.NombreComercial,
                request.Direccion,
                request.FlagIGV,
                request.CreditoMaximo,
                request.IdUsuarioModificador,
                DateTime.Now);

            if (updateResult.IsFailure)
            {
                return updateResult;
            }

            await clientRepository.UpdateAsync(client, cancellationToken);

            return Result.Success();
        }
    }
}
