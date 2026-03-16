using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Clients.Queries.GetClientById
{
    public sealed record ClientDetailResponse(
        int IdCliente,
        string Nombre,
        string? NombreComercial,
        EDocumentoIdentidad TipoDocumento,
        string TipoDocumentoDescripcion,
        string NumDocumento,
        string? CodValidadorDoc,
        int IdPais,
        string Direccion,
        bool FlagIGV,
        decimal CreditoMaximo,
        EEstado Estado,
        DateTime FechaAlta,
        DateTime? FechaBaja);
}
