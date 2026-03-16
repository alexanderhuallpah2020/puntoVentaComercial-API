namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    /// <summary>
    /// Valida el DNI peruano (Documento Nacional de Identidad).
    /// Algoritmo: módulo 11 con pesos [3,2,7,6,5,4,3,2] sobre los 8 dígitos base.
    /// El dígito verificador puede ser numérico o las letras K y J.
    /// </summary>
    public static class DniValidator
    {
        private static readonly int[] Weights = [3, 2, 7, 6, 5, 4, 3, 2];

        private static readonly char[] CheckTable = ['1', '0', '5', '4', '3', '2', 'K', 'J', '8', '7', '6'];

        /// <summary>
        /// Valida el DNI peruano.
        /// </summary>
        /// <param name="dniBase8">Los 8 dígitos numéricos del DNI.</param>
        /// <param name="checkDigit">El dígito verificador (puede ser 0-9, K o J).</param>
        public static bool Validate(string dniBase8, char checkDigit)
        {
            if (string.IsNullOrEmpty(dniBase8) || dniBase8.Length != 8)
                return false;

            foreach (char c in dniBase8)
            {
                if (!char.IsDigit(c)) return false;
            }

            int sum = 0;
            for (int i = 0; i < 8; i++)
            {
                sum += (dniBase8[i] - '0') * Weights[i];
            }

            int remainder = sum % 11;
            char expected = CheckTable[remainder];

            return char.ToUpper(checkDigit) == expected;
        }
    }
}
