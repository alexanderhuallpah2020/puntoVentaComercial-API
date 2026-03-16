using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    public static class ClientErrors
    {
        public static Error NotFound(int id) =>
            Error.NotFound("Cliente.NotFound", $"El cliente con Id {id} no existe.");

        public static readonly Error RucInvalido =
            Error.Failure("Cliente.RucInvalido", "El número de RUC no es válido.");

        public static readonly Error DniInvalido =
            Error.Failure("Cliente.DniInvalido", "El número de DNI o su código verificador no son válidos.");

        public static readonly Error DocumentoDuplicado =
            Error.Conflict("Cliente.DocumentoDuplicado", "Ya existe un cliente registrado con ese número de documento.");

        public static readonly Error NombreRequerido =
            Error.Failure("Cliente.NombreRequerido", "El nombre del cliente es obligatorio.");

        public static readonly Error DireccionRequerida =
            Error.Failure("Cliente.DireccionRequerida", "La dirección del cliente es obligatoria.");
    }
}
