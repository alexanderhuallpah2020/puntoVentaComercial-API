using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Commands.CreateSegmentoSunat
{
    public sealed record CreateSegmentoSunatCommand(
           string Codigo,
           string Descripcion
       ) : ICommand<int>;
}
