using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

public static class ClienteErrors
{
    public static Error NotFound(int id) =>
        Error.NotFound("Cliente.NotFound", $"El cliente con ID {id} no existe.");

    public static Error DocumentoDuplicado(string numDoc) =>
        Error.Conflict("Cliente.DocumentoDuplicado", $"Ya existe un cliente con el documento {numDoc}.");

    public static Error DniInvalido(string numDoc) =>
        Error.Problem("Cliente.DniInvalido", $"El DNI {numDoc} con su código validador no es válido.");

    public static Error RucInvalido(string ruc) =>
        Error.Problem("Cliente.RucInvalido", $"El RUC {ruc} no es válido.");

    public static readonly Error NoEditableDesdePos =
        Error.Problem("Cliente.NoEditableDesdePos", "Este cliente fue creado desde mantenimiento de trabajadores y no puede modificarse desde el POS.");

    public static readonly Error PaisRequerido =
        Error.Problem("Cliente.PaisRequerido", "Debe especificar el país de origen.");

    public static readonly Error NombreRequerido =
        Error.Problem("Cliente.NombreRequerido", "Debe ingresar el nombre del cliente.");

    public static readonly Error DireccionRequerida =
        Error.Problem("Cliente.DireccionRequerida", "Debe ingresar la dirección del cliente.");

    public static readonly Error SunatNoDisponible =
        Error.Problem("Cliente.SunatNoDisponible", "El servicio de consulta SUNAT no está disponible en este momento.");

    public static Error DocumentoIdentidadNoEncontrado(int id) =>
        Error.NotFound("Cliente.DocumentoIdentidadNoEncontrado", $"El tipo de documento con ID {id} no existe.");

    public static Error PaisNoEncontrado(short id) =>
        Error.NotFound("Cliente.PaisNoEncontrado", $"El país con ID {id} no existe.");
}
