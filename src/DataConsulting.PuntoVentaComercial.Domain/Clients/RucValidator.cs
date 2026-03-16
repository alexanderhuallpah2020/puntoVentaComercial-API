namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    /// <summary>
    /// Valida el RUC peruano (Registro Único de Contribuyentes).
    /// Algoritmo: módulo 11 con pesos [5,4,3,2,7,6,5,4,3,2] sobre los primeros 10 dígitos.
    /// El 11vo dígito es el verificador.
    /// </summary>
    public static class RucValidator
    {
        private static readonly int[] Weights = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];

        /// <summary>
        /// Valida el RUC peruano de 11 dígitos.
        /// </summary>
        public static bool Validate(string ruc)
        {
            if (string.IsNullOrEmpty(ruc) || ruc.Length != 11)
                return false;

            foreach (char c in ruc)
            {
                if (!char.IsDigit(c)) return false;
            }

            int sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (ruc[i] - '0') * Weights[i];
            }

            int remainder = sum % 11;
            int complement = 11 - remainder;

            // Ajuste: si complement es 11 → 0; si complement es 10 → 1
            int checkDigit = complement switch
            {
                11 => 0,
                10 => 1,
                _ => complement
            };

            return (ruc[10] - '0') == checkDigit;
        }
    }
}
