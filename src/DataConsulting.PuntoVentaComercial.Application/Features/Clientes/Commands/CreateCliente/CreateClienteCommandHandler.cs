using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Domain.DocumentosIdentidad;
using DataConsulting.PuntoVentaComercial.Domain.Paises;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clientes.Commands.CreateCliente;

internal sealed class CreateClienteCommandHandler(
    IClienteRepository repository,
    IDocumentoIdentidadRepository documentoIdentidadRepository,
    IPaisRepository paisRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateClienteCommand, int>
{
    public async Task<Result<int>> Handle(
        CreateClienteCommand request, CancellationToken cancellationToken)
    {
        if (request.IdDocumentoIdentidad.HasValue)
        {
            bool existeDoc = await documentoIdentidadRepository.ExistsAsync(
                request.IdDocumentoIdentidad.Value, cancellationToken);
            if (!existeDoc)
                return Result.Failure<int>(ClienteErrors.DocumentoIdentidadNoEncontrado(request.IdDocumentoIdentidad.Value));
        }

        bool existePais = await paisRepository.ExistsAsync(request.IdPais, cancellationToken);
        if (!existePais)
            return Result.Failure<int>(ClienteErrors.PaisNoEncontrado(request.IdPais));

        if (request.IdDocumentoIdentidad.HasValue && !string.IsNullOrWhiteSpace(request.NumDocumento))
        {
            bool existe = await repository.ExistsByDocumentoAsync(
                request.IdDocumentoIdentidad.Value, request.NumDocumento, cancellationToken);
            if (existe)
                return Result.Failure<int>(ClienteErrors.DocumentoDuplicado(request.NumDocumento));
        }

        int nuevoLocalId      = await repository.GetNextLocalIdAsync(cancellationToken);
        int nuevoLocalUnicoId = await repository.GetNextLocalUnicoIdAsync(cancellationToken);

        var result = Cliente.Create(
            request.Nombre,
            request.IdDocumentoIdentidad,
            request.NumDocumento,
            request.CodValidadorDoc,
            request.IdPais,
            request.DireccionLocal,
            request.Telefono1,
            request.IdSucursal,
            idLocal: nuevoLocalId,
            idLocalUnico: nuevoLocalUnicoId,
            usuarioCreador: "SISTEMA");

        if (result.IsFailure)
            return Result.Failure<int>(result.Error);

        repository.Add(result.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
