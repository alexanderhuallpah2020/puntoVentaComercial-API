using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.UpdateClient
{
    internal sealed class UpdateClientCommandHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<UpdateClientCommand>
    {
        public async Task<Result> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            var client = await clientRepository.GetByIdAsync(request.IdCliente, cancellationToken);
            if (client is null)
                return Result.Failure(ClientErrors.ClienteNoEncontrado);

            var updateResult = client.Update(
                request.Nombre,
                request.CodValidadorDoc,
                request.IdUsuarioModificador,
                DateTime.Now);

            if (updateResult.IsFailure)
                return updateResult;

            client.UpdateLocal(request.Direccion, request.Telefono);

            clientRepository.Update(client);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
