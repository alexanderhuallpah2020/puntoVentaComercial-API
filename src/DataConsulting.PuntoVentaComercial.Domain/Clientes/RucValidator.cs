namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

public static class RucValidator
{
    private static readonly int[] Factores = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];

    public static bool EsValido(string ruc)
    {
        if (string.IsNullOrWhiteSpace(ruc) || ruc.Length != 11)
            return false;

        if (!ruc.All(char.IsDigit))
            return false;

        string prefijo = ruc[..2];
        if (prefijo != "10" && prefijo != "20")
            return false;

        int suma = 0;
        for (int i = 0; i < 10; i++)
            suma += (ruc[i] - '0') * Factores[i];

        int digitoVerificador = 11 - (suma % 11);
        if (digitoVerificador == 11)
            digitoVerificador = 0;
        else if (digitoVerificador == 10)
            digitoVerificador = 1;

        return digitoVerificador == (ruc[10] - '0');
    }

    public static int ObtenerDigitoVerificador(string ruc10)
    {
        if (string.IsNullOrWhiteSpace(ruc10) || ruc10.Length != 10)
            return -1;

        if (!ruc10.All(char.IsDigit))
            return -1;

        int suma = 0;
        for (int i = 0; i < 10; i++)
            suma += (ruc10[i] - '0') * Factores[i];

        int digitoVerificador = 11 - (suma % 11);
        if (digitoVerificador == 11)
            digitoVerificador = 0;
        else if (digitoVerificador == 10)
            digitoVerificador = 1;

        return digitoVerificador;
    }
}