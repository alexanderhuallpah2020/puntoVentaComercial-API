using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Services.ClasesSunat;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Application.Features.ClasesSunat.Commands.CreateClaseSunat
{
    internal sealed class CreateClaseSunatCommandHandler(
       IClaseSunatService claseService,
       IUnitOfWork unitOfWork) : ICommandHandler<CreateClaseSunatCommand, int>
    {
        public async Task<Result<int>> Handle(CreateClaseSunatCommand request, CancellationToken ct)
        {
            var result = await claseService.RegistrarClaseAsync(
                request.IdFamiliaSunat,
                request.Codigo,
                request.Descripcion,
                request.IdUsuarioCreador,
                ct);

            if (result.IsFailure)
            {
                return result;
            }

            await unitOfWork.SaveChangesAsync(ct);

            return result;
        }
    }
}
