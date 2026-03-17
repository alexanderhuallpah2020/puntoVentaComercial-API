using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sales.Queries.GetSales
{
    public sealed record GetSalesQuery(
        int IdEmpresa,
        int IdSucursal,
        DateTime? FechaDesde,
        DateTime? FechaHasta,
        int? IdTipoDocumento,
        string? NumSerie,
        long? Correlativo,
        int? IdCliente,
        int? Estado,
        int PageSize = 100
    ) : IQuery<GetSalesResponse>;
}
