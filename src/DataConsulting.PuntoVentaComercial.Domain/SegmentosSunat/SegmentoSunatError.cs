using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.SegmentosSunat
{
    public static class SegmentoSunatErrors
    {
        public static Error NotFound(int idSegmentoSunat) =>
            Error.NotFound(
                "SegmentosSunat.NotFound",
                $"No se encontró el SegmentoSunat con Id {idSegmentoSunat}");

        public static Error CodigoDuplicado(string codigo) =>
            Error.Conflict(
                "SegmentosSunat.CodigoDuplicado",
                $"Ya existe un SegmentoSunat con código '{codigo}'");
    }
}
