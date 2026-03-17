namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    /// <summary>
    /// Valida el RUC peruano (11 dígitos, módulo 11).
    /// Equivalente a UIFunctions.ValidarRUC del legacy.
    ///
    /// Algoritmo:
    ///   - Pesos: [5, 4, 3, 2, 7, 6, 5, 4, 3, 2] sobre los primeros 10 dígitos
    ///   - suma = sumatoria de (dígito[i] × peso[i])
    ///   - R = suma % 11
    ///   - digito_esperado = 11 - R
    ///   - Si digito_esperado == 11 → 0
    ///   - Si digito_esperado == 10 → 1
    ///   - El dígito 11 del RUC debe coincidir con digito_esperado
    ///
    /// Prefijos válidos según SUNAT: 10 (persona natural), 15, 17, 20 (empresa)
    /// </summary>
    public static class RucValidator
    {
        private const int RucLength = 11;
        private static readonly int[] Weights = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];

        private static readonly string[] ValidPrefixes = ["10", "15", "17", "20"];

        public static bool Validate(string ruc)
        {
            if (string.IsNullOrWhiteSpace(ruc) || ruc.Length != RucLength)
                return false;

            if (!ruc.All(char.IsDigit))
                return false;

            var prefix = ruc[..2];
            if (!ValidPrefixes.Contains(prefix))
                return false;

            int sum = 0;
            for (int i = 0; i < RucLength - 1; i++)
                sum += (ruc[i] - '0') * Weights[i];

            int r = sum % 11;
            int expected = 11 - r;

            if (expected == 11) expected = 0;
            else if (expected == 10) expected = 1;

            return (ruc[10] - '0') == expected;
        }

        /// <summary>
        /// Determina si el RUC corresponde a empresa (prefijo 20) o persona natural (prefijo 10/15/17).
        /// Importante para determinar si corresponde Factura o Boleta.
        /// </summary>
        public static bool EsRucEmpresa(string ruc) =>
            !string.IsNullOrWhiteSpace(ruc) && ruc.Length == RucLength && ruc.StartsWith("20");
    }
}
