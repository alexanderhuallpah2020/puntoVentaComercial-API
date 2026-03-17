using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    public static class ClientErrors
    {
        public static readonly Error NombreRequerido =
            Error.Failure("Client.NombreRequerido", "El nombre del cliente es obligatorio.");

        public static readonly Error PaisRequerido =
            Error.Failure("Client.PaisRequerido", "Debe especificar el país de origen del cliente.");

        public static readonly Error TipoDocumentoRequerido =
            Error.Failure("Client.TipoDocumentoRequerido", "Debe especificar el tipo de documento de identidad.");

        public static readonly Error NumDocumentoRequerido =
            Error.Failure("Client.NumDocumentoRequerido", "Debe ingresar el número del documento.");

        public static readonly Error DireccionRequerida =
            Error.Failure("Client.DireccionRequerida", "Debe ingresar la dirección del cliente.");

        public static readonly Error CodValidadorDniRequerido =
            Error.Failure("Client.CodValidadorDniRequerido", "Ingrese el código validador para el DNI.");

        public static readonly Error DniInvalido =
            Error.Failure("Client.DniInvalido", "El número de DNI o su código validador no es válido.");

        public static readonly Error RucInvalido =
            Error.Failure("Client.RucInvalido", "El número de RUC no es válido.");

        public static readonly Error RucLongitudInvalida =
            Error.Failure("Client.RucLongitudInvalida", "El RUC debe tener exactamente 11 dígitos.");

        public static readonly Error DniLongitudInvalida =
            Error.Failure("Client.DniLongitudInvalida", "El DNI debe tener exactamente 8 dígitos.");

        public static readonly Error ClienteNoEncontrado =
            Error.NotFound("Client.NoEncontrado", "No se encontró el cliente especificado.");

        public static readonly Error RucNoEncontradoEnSunat =
            Error.NotFound("Client.RucNoEncontradoEnSunat", "No se encontró información del RUC en SUNAT.");

        public static readonly Error SunatNoDisponible =
            Error.Problem("Client.SunatNoDisponible", "El servicio de consulta SUNAT no está disponible en este momento.");

        // Regla de negocio: factura requiere RUC (blueprint regla #12)
        public static readonly Error FacturaRequiereRuc =
            Error.Failure("Client.FacturaRequiereRuc", "Para emitir factura el cliente debe tener RUC.");
    }
}
