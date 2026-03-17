namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    /// <summary>
    /// Valida el DNI peruano con su dígito verificador.
    /// Equivalente a FunctionsBR.ValidarDNIPeru del legacy.
    ///
    /// Algoritmo (módulo 11):
    ///   - Pesos: [3, 2, 7, 6, 5, 4, 3, 2] sobre los 8 dígitos del DNI
    ///   - suma = sumatoria de (dígito[i] × peso[i])
    ///   - R = 11 - (suma % 11)
    ///   - Tabla: R=11 → '1', R=10 → 'K', resto → R.ToString()
    ///   - El dígito verificador (CodValidadorDoc) debe coincidir con la tabla
    /// </summary>
    public static class DniValidator
    {
        private const int DniLength = 8;
        private static readonly int[] Weights = [3, 2, 7, 6, 5, 4, 3, 2];

        /// <summary>
        /// Valida el DNI más su código verificador.
        /// </summary>
        /// <param name="dni">8 dígitos del DNI</param>
        /// <param name="codValidador">1 carácter verificador (0-9 o K)</param>
        public static bool Validate(string dni, string codValidador)
        {
            if (string.IsNullOrWhiteSpace(dni) || dni.Length != DniLength)
                return false;

            if (!dni.All(char.IsDigit))
                return false;

            if (string.IsNullOrWhiteSpace(codValidador) || codValidador.Length != 1)
                return false;

            int sum = 0;
            for (int i = 0; i < DniLength; i++)
                sum += (dni[i] - '0') * Weights[i];

            int r = 11 - (sum % 11);
            string expected = r == 11 ? "1" : r == 10 ? "K" : r.ToString();

            return string.Equals(codValidador.Trim(), expected, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Calcula el dígito verificador esperado para un DNI de 8 dígitos.
        /// Útil para pruebas y para informar al usuario cuál es el código correcto.
        /// </summary>
        public static string? GetExpectedCheckDigit(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni) || dni.Length != DniLength || !dni.All(char.IsDigit))
                return null;

            int sum = 0;
            for (int i = 0; i < DniLength; i++)
                sum += (dni[i] - '0') * Weights[i];

            int r = 11 - (sum % 11);
            return r == 11 ? "1" : r == 10 ? "K" : r.ToString();
        }
    }
}
