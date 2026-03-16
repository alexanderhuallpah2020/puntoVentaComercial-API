using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetSellers
{
    public sealed record GetSellersQuery(int IdEmpresa) : IQuery<GetSellersResponse>;
}
