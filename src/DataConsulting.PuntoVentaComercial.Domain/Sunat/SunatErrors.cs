using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Sunat
{
    public static class SunatErrors
    {
        public static Error NotFound(int idVenta) =>
            Error.NotFound("Sunat.NotFound", $"No existe envío SUNAT para la venta {idVenta}.");

        public static readonly Error VentaNoEncontrada =
            Error.NotFound("Sunat.VentaNoEncontrada", "La venta especificada no existe.");

        public static readonly Error YaAceptada =
            Error.Conflict("Sunat.YaAceptada", "El comprobante ya fue aceptado por SUNAT.");

        public static readonly Error EmpresaSinRuc =
            Error.Failure("Sunat.EmpresaSinRuc", "No se encontró el RUC de la empresa emisora.");

        public static Error RechazadoPorSunat(string codigo, string mensaje) =>
            Error.Failure("Sunat.Rechazado", $"SUNAT rechazó el comprobante [{codigo}]: {mensaje}");

        public static readonly Error ServicioNoDisponible =
            Error.Failure("Sunat.ServicioNoDisponible",
                "El servicio de SUNAT no está disponible. Intente nuevamente.");
    }
}
