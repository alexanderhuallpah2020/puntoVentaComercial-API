using DataConsulting.PuntoVentaComercial.Domain.Abstractions;

namespace DataConsulting.PuntoVentaComercial.Domain.Cash
{
    public static class CashErrors
    {
        public static readonly Error ImporteInvalido =
            Error.Failure("Deposito.ImporteInvalido", "El importe del depósito debe ser mayor a cero.");

        public static readonly Error SerieRequerida =
            Error.Failure("Deposito.SerieRequerida", "Debe especificar la serie del documento.");

        public static Error NotFound(int id) =>
            Error.NotFound("Deposito.NotFound", $"El depósito {id} no existe.");

        public static Error DepositoYaAnulado(int id) =>
            Error.Conflict("Deposito.YaAnulado", $"El depósito {id} ya fue anulado.");
    }
}
