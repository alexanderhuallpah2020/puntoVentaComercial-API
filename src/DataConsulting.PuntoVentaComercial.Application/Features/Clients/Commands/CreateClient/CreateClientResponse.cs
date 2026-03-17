namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Commands.CreateClient
{
    public sealed record CreateClientResponse(
        int IdCliente,
        string Nombre,
        string NumDocumento,
        int IdDocumentoIdentidad);
}
