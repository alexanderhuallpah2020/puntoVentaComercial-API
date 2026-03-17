namespace DataConsulting.PuntoVentaComercial.API.Controllers.Clients
{
    public sealed record UpdateClientRequest(
        string Nombre,
        string CodValidadorDoc,
        string Direccion,
        string Telefono,
        int IdSucursal,
        int IdUsuarioModificador);
}
