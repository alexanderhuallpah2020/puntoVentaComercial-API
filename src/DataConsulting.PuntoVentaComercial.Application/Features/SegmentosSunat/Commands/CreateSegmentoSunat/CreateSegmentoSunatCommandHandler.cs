using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.SegmentosSunat;

namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Commands.CreateSegmentoSunat
{
    internal sealed class CreateSegmentoSunatCommandHandler(
         ISegmentoSunatRepository repository,
         IUnitOfWork unitOfWork)
         : ICommandHandler<CreateSegmentoSunatCommand, int>
    {
        public async Task<Result<int>> Handle(CreateSegmentoSunatCommand request, CancellationToken cancellationToken)
        {
            if (await repository.ExistsByCodigoAsync(request.Codigo, cancellationToken))
            {
                return Result.Failure<int>(SegmentoSunatErrors.CodigoDuplicado(request.Codigo));
            }

            int nuevoId = await repository.GetNextIdAsync(cancellationToken);

            var result = SegmentoSunat.Create(
               nuevoId,
               request.Codigo,
               request.Descripcion,
               1,
               1,
               DateTime.UtcNow);

            if (result.IsFailure)
            {
                return Result.Failure<int>(result.Error);
            }

            repository.Add(result.Value);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(result.Value.Id);
        }
    }
}
