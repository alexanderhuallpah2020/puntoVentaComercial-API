using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Clients
{
    public sealed record CreateClientRequest(
        string Nombre,
        EDocumentoIdentidad IdDocumentoIdentidad,
        string NumDocumento,
        string CodValidadorDoc,
        int IdPais,
        int IdSucursal,
        string Direccion,
        string Telefono,
        int IdUsuarioCreador);
}
