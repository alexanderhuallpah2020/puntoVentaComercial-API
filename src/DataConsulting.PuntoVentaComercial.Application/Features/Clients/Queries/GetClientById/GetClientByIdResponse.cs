namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.GetClientById
{
    public sealed record GetClientByIdResponse(
        int IdCliente,
        string Nombre,
        string NombreComercial,
        int IdDocumentoIdentidad,
        string NumDocumento,
        string CodValidadorDoc,
        int IdPais,
        int IdTipoCliente,
        bool FlagIGV,
        decimal CreditoMaximo,
        string EstadoCliente);
}
