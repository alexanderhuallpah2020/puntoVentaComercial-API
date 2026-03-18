using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Ventas;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Ventas.Commands.AnularVenta;

internal sealed class AnularVentaCommandHandler(
    IVentaRepository repository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<AnularVentaCommand, bool>
{
    public async Task<Result<bool>> Handle(
        AnularVentaCommand request, CancellationToken cancellationToken)
    {
        var venta = await repository.GetByIdAsync(request.IdVenta, cancellationToken);
        if (venta is null)
            return Result.Failure<bool>(VentaErrors.NotFound(request.IdVenta));

        var result = venta.Anular("admin");
        if (result.IsFailure)
            return Result.Failure<bool>(result.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }
}
