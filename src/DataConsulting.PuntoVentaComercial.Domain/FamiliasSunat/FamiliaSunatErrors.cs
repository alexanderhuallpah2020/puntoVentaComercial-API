using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.FamiliasSunat
{
    public static class FamiliaSunatErrors
    {
        public static Error NotFound(int idFamiliaSunat) =>
            Error.NotFound(
                "FamiliasSunat.NotFound",
                $"No se encontró la FamiliaSunat con Id {idFamiliaSunat}");

        public static Error SegmentoNotFound(int idSegmentoSunat) =>
            Error.NotFound(
                "FamiliasSunat.SegmentoNotFound",
                $"No se encontró el SegmentoSunat con Id {idSegmentoSunat}");

        public static Error CodigoDuplicado(int idSegmentoSunat, string codigo) =>
            Error.Conflict(
                "FamiliasSunat.CodigoDuplicado",
                $"Ya existe una FamiliaSunat con código '{codigo}' para el SegmentoSunat {idSegmentoSunat}");
    }
}
