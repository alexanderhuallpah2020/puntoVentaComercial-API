namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Settings;

public sealed class SunatSettings
{
    public const string SectionName = "Sunat";

    public string Ambiente { get; init; } = "pruebas";
    public string RutaCertificados { get; init; } = string.Empty;
    public string RutaPlantillas { get; init; } = string.Empty;

    public bool EsPruebas =>
        !string.Equals(Ambiente, "produccion", StringComparison.OrdinalIgnoreCase);
}
