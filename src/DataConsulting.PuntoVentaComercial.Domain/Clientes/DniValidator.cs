namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

public static class DniValidator
{
    private static readonly int[] Factores = [3, 2, 7, 6, 5, 4, 3, 2];
    private static readonly char[] Resultados = ['6', '7', '8', '9', '0', '1', '1', '2', '3', '4', '5'];

    public static bool EsValido(string numDni, string codValidador)
    {
        if (string.IsNullOrWhiteSpace(numDni) || numDni.Length != 8)
            return false;

        if (!numDni.All(char.IsDigit))
            return false;

        if (string.IsNullOrWhiteSpace(codValidador) || codValidador.Length != 1)
            return false;

        char esperado = ObtenerCodValidador(numDni);
        if (esperado == '\0')
            return false;

        return esperado == char.ToUpperInvariant(codValidador[0]);
    }

    public static char ObtenerCodValidador(string numDni)
    {
        if (string.IsNullOrWhiteSpace(numDni) || numDni.Length != 8)
            return '\0';

        if (!numDni.All(char.IsDigit))
            return '\0';

        int suma = 0;
        for (int i = 0; i < 8; i++)
            suma += (numDni[i] - '0') * Factores[i];

        int indice = 11 - (suma % 11);
        if (indice == 11)
            indice = 0;

        return Resultados[indice];
    }
}