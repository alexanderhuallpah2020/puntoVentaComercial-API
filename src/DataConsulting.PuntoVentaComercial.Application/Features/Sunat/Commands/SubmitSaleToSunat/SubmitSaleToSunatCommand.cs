using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Sunat.Commands.SubmitSaleToSunat
{
    public sealed record SubmitSaleToSunatCommand(int IdVenta) : ICommand<SubmitSaleToSunatResponse>;
}
