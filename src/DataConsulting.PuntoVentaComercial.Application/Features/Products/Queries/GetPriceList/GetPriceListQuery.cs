using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetPriceList
{
    public sealed record GetPriceListQuery(
        int IdSucursal,
        int IdTipoCliente,
        string? Codigo,
        string? Descripcion,
        bool SoloConStock
    ) : IQuery<GetPriceListResponse>;
}
