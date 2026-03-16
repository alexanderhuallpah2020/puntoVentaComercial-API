using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoSunatById
{
    public sealed record GetSegmentoSunatByIdQuery(int idSegmentoSunat)
            : IQuery<GetSegmentoSunatByIdResponse>;
}
