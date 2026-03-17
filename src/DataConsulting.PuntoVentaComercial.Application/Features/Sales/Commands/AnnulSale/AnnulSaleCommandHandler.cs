using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Sales;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.AnnulSale
{
    internal sealed class AnnulSaleCommandHandler(
        ISaleRepository saleRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<AnnulSaleCommand>
    {
        public async Task<Result> Handle(AnnulSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await saleRepository.GetByIdAsync(request.IdVenta, cancellationToken);
            if (sale is null)
                return Result.Failure(SaleErrors.NotFound(request.IdVenta));

            var annulResult = sale.Annul(request.IdUsuarioModificador, DateTime.Now);
            if (annulResult.IsFailure)
                return annulResult;

            saleRepository.Update(sale);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
