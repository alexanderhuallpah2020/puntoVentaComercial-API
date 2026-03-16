using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    public static class ConfigurationErrors
    {
        public static readonly Error SerieNoEncontrada =
            new("Config.SerieNoEncontrada", "No se encontró la serie para los parámetros indicados.", ErrorType.NotFound);

        public static readonly Error SinTipoCambio =
            new("Config.SinTipoCambio", "No hay tipo de cambio registrado para la fecha indicada.", ErrorType.NotFound);

        public static readonly Error SinTurnos =
            new("Config.SinTurnos", "No hay turnos activos configurados para este horario.", ErrorType.NotFound);
    }
}
