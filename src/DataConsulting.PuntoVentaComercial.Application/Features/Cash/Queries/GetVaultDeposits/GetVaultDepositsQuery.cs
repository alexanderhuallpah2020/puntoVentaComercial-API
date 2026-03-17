using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Queries.GetVaultDeposits
{
    public sealed record GetVaultDepositsQuery(
        int IdEmpresa,
        int IdSucursal,
        int? IdIsla,
        int? IdTrabajador,
        EDocumento? TipoDocumento,
        string? NumSerie,
        string? NumDocumento,
        ETipoMoneda? TipoMoneda,
        DateTime? FechaDesde,
        DateTime? FechaHasta,
        int PageSize = 100
    ) : IQuery<GetVaultDepositsResponse>;
}
