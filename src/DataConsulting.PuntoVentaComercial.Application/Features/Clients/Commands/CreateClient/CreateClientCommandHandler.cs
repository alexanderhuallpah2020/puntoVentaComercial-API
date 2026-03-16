using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clients;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.CreateClient
{
    internal sealed class CreateClientCommandHandler(
        IClientRepository clientRepository)
        : ICommandHandler<CreateClientCommand, int>
    {
        public async Task<Result<int>> Handle(CreateClientCommand request, CancellationToken ct)
        {
            bool exists = await clientRepository.ExistsByDocumentoAsync(
                request.IdDocumentoIdentidad,
                request.NumDocumento,
                ct);

            if (exists)
            {
                return Result.Failure<int>(ClientErrors.DocumentoDuplicado);
            }

            Result<Client> clientResult = Client.Create(
                request.Nombre,
                request.NombreComercial,
                request.IdDocumentoIdentidad,
                request.NumDocumento,
                request.CodValidadorDoc,
                request.IdPais,
                request.Direccion,
                request.FlagIGV,
                request.CreditoMaximo,
                request.IdUsuarioCreador,
                DateTime.Now);

            if (clientResult.IsFailure)
            {
                return Result.Failure<int>(clientResult.Error);
            }

            int idCliente = await clientRepository.InsertAsync(
                clientResult.Value,
                request.Telefono1,
                request.IdSucursal,
                ct);

            return Result.Success(idCliente);
        }
    }
}
