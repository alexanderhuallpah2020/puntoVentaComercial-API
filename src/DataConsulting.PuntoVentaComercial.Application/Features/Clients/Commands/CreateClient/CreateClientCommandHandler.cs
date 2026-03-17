using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.CreateClient
{
    internal sealed class CreateClientCommandHandler(
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<CreateClientCommand, CreateClientResponse>
    {
        public async Task<Result<CreateClientResponse>> Handle(
            CreateClientCommand request,
            CancellationToken cancellationToken)
        {
            var exists = await clientRepository.ExistsByDocumentoAsync(
                (int)request.IdDocumentoIdentidad,
                request.NumDocumento,
                excludeIdCliente: null,
                cancellationToken);

            if (exists)
                return Result.Failure<CreateClientResponse>(ClientErrors.DniInvalido);

            var fechaCreacion = DateTime.Now;

            var clientResult = Client.Create(
                request.Nombre,
                request.IdDocumentoIdentidad,
                request.NumDocumento,
                request.CodValidadorDoc,
                request.IdPais,
                idTipoCliente: 1,
                request.IdUsuarioCreador,
                fechaCreacion);

            if (clientResult.IsFailure)
                return Result.Failure<CreateClientResponse>(clientResult.Error);

            var client = clientResult.Value;

            var local = ClientLocal.Create(
                idCliente: 0,
                request.IdSucursal,
                request.Direccion,
                request.Telefono);

            client.AddLocal(local);

            clientRepository.Add(client);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new CreateClientResponse(
                client.IdCliente,
                client.Nombre,
                client.NumDocumento,
                (int)client.IdDocumentoIdentidad));
        }
    }
}
