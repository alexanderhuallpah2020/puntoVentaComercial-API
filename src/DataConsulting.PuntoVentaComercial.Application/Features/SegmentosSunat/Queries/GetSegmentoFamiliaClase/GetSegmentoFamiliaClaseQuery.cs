using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase
{
    public sealed record GetSegmentoFamiliaClaseQuery()
           : IQuery<List<GetSegmentoFamiliaClaseResponse>>;
}
