using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetAllSegmentosSunat
{
    public sealed record GetAllSegmentosSunatQuery() : IQuery<List<GetAllSegmentosSunatResponse>>;
}
