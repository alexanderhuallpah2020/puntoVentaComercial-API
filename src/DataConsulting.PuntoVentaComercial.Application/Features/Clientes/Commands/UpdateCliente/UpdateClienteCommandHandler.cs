using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Domain.DocumentosIdentidad;
using DataConsulting.PuntoVentaComercial.Domain.Paises;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Commands.UpdateCliente;

internal sealed class UpdateClienteCommandHandler(
    IClienteRepository repository,
    IDocumentoIdentidadRepository documentoIdentidadRepository,
    IPaisRepository paisRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateClienteCommand, bool>
{
    public async Task<Result<bool>> Handle(
        UpdateClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await repository.GetByIdAsync(request.IdCliente, cancellationToken);
        if (cliente is null)
            return Result.Failure<bool>(ClienteErrors.NotFound(request.IdCliente));

        if (request.IdDocumentoIdentidad.HasValue)
        {
            bool existeDoc = await documentoIdentidadRepository.ExistsAsync(
                request.IdDocumentoIdentidad.Value, cancellationToken);
            if (!existeDoc)
                return Result.Failure<bool>(ClienteErrors.DocumentoIdentidadNoEncontrado(request.IdDocumentoIdentidad.Value));
        }

        bool existePais = await paisRepository.ExistsAsync(request.IdPais, cancellationToken);
        if (!existePais)
            return Result.Failure<bool>(ClienteErrors.PaisNoEncontrado(request.IdPais));

        var result = cliente.Update(
            request.Nombre,
            request.IdDocumentoIdentidad,
            request.NumDocumento,
            request.CodValidadorDoc,
            request.IdPais,
            request.DireccionLocal,
            request.Telefono1,
            usuarioModificador: "SISTEMA");

        if (result.IsFailure)
            return Result.Failure<bool>(result.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
