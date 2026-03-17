using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Cash
{
    public sealed class VaultDeposit : Entity
    {
        // ORM constructor
        private VaultDeposit() { }

        public int IdDepositoBoveda => Id;
        public int IdEmpresa { get; private set; }
        public int IdSucursal { get; private set; }
        public int IdTrabajador { get; private set; }
        public int IdIsla { get; private set; }
        public int IdTurnoAsistencia { get; private set; }
        public DateTime FechaEmision { get; private set; }
        public EDocumento TipoDocumento { get; private set; }
        public string NumSerie { get; private set; } = string.Empty;
        public string NumDocumento { get; private set; } = string.Empty;
        public ETipoMoneda TipoMoneda { get; private set; }
        public decimal TipoCambio { get; private set; }
        public decimal Importe { get; private set; }
        public string? Glosa { get; private set; }
        public int IdFormaPago { get; private set; }
        public int Estado { get; private set; } = 1;
        public short UpdateToken { get; private set; }
        public int IdUsuarioCreador { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public int? IdUsuarioModificador { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        public static Result<VaultDeposit> Create(
            int idEmpresa,
            int idSucursal,
            int idTrabajador,
            int idIsla,
            int idTurnoAsistencia,
            EDocumento tipoDocumento,
            string numSerie,
            string numDocumento,
            ETipoMoneda tipoMoneda,
            decimal tipoCambio,
            decimal importe,
            int idFormaPago,
            string? glosa,
            int idUsuarioCreador,
            DateTime fechaCreacion)
        {
            if (importe <= 0)
                return Result.Failure<VaultDeposit>(CashErrors.ImporteInvalido);

            if (string.IsNullOrWhiteSpace(numSerie))
                return Result.Failure<VaultDeposit>(CashErrors.SerieRequerida);

            var deposit = new VaultDeposit
            {
                IdEmpresa = idEmpresa,
                IdSucursal = idSucursal,
                IdTrabajador = idTrabajador,
                IdIsla = idIsla,
                IdTurnoAsistencia = idTurnoAsistencia,
                FechaEmision = fechaCreacion,
                TipoDocumento = tipoDocumento,
                NumSerie = numSerie.Trim(),
                NumDocumento = numDocumento?.Trim() ?? string.Empty,
                TipoMoneda = tipoMoneda,
                TipoCambio = tipoCambio <= 0 ? 1m : tipoCambio,
                Importe = importe,
                IdFormaPago = idFormaPago,
                Glosa = glosa,
                Estado = 1,
                UpdateToken = 1,
                IdUsuarioCreador = idUsuarioCreador,
                FechaCreacion = fechaCreacion
            };

            return Result.Success(deposit);
        }

        public Result Annul(int idUsuarioModificador, DateTime fechaModificacion)
        {
            if (Estado == 0)
                return Result.Failure(CashErrors.DepositoYaAnulado(IdDepositoBoveda));

            Estado = 0;
            IdUsuarioModificador = idUsuarioModificador;
            FechaModificacion = fechaModificacion;
            UpdateToken++;

            return Result.Success();
        }
    }
}
