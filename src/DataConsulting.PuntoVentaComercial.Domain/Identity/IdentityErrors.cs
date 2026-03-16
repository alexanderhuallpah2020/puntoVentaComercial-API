using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Identity
{
    public static class IdentityErrors
    {
        public static readonly Error CredencialesInvalidas =
            Error.Failure("Auth.CredencialesInvalidas", "Usuario o contraseña incorrectos.");

        public static readonly Error UsuarioInactivo =
            Error.Failure("Auth.UsuarioInactivo", "El usuario se encuentra inactivo.");

        public static readonly Error EstacionNoEncontrada =
            Error.Failure("Auth.EstacionNoEncontrada", "La estación indicada no está registrada en esta empresa.");

        public static readonly Error EstacionInactiva =
            Error.Failure("Auth.EstacionInactiva", "La estación indicada está inactiva.");

        public static readonly Error TokenInvalido =
            Error.Failure("Auth.TokenInvalido", "El token de acceso no es válido o expiró.");
    }
}
