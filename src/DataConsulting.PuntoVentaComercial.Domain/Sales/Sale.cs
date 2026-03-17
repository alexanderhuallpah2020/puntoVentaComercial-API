using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Sales
{
    public sealed class Sale : Entity
    {
        private readonly List<SaleItem> _items = [];

        // ORM constructor
        private Sale() { }

        public int IdVenta => Id;
        public int IdEmpresa { get; private set; }
        public int IdSucursal { get; private set; }
        public int IdEstacion { get; private set; }
        public int IdTurnoAsistencia { get; private set; }
        public DateTime FechaEmision { get; private set; }
        public EDocumento TipoDocumento { get; private set; }
        public string NumSerie { get; private set; } = string.Empty;
        public long Correlativo { get; private set; }

        // Cliente denormalizado (igual que el legacy)
        public int IdCliente { get; private set; }
        public string NombreCliente { get; private set; } = string.Empty;
        public int IdDocumentoIdentidad { get; private set; }
        public string NumDocumentoCliente { get; private set; } = string.Empty;
        public string DireccionCliente { get; private set; } = string.Empty;
        public bool FlagIGV { get; private set; }

        public int IdTrabajador { get; private set; }
        public int? IdTrabajador2 { get; private set; }
        public EFormaPago FormaPago { get; private set; }
        public ETipoMoneda TipoMoneda { get; private set; }
        public decimal TipoCambio { get; private set; }

        public decimal DescuentoGlobal { get; private set; }

        // Totales
        public decimal ValorAfecto { get; private set; }
        public decimal ValorInafecto { get; private set; }
        public decimal ValorExonerado { get; private set; }
        public decimal ValorGratuito { get; private set; }
        public decimal TotalIsc { get; private set; }
        public decimal Igv { get; private set; }
        public decimal TotalIcbper { get; private set; }
        public decimal ImporteTotal { get; private set; }

        public bool FlagDetraccion { get; private set; }
        public int? IdSubdiario { get; private set; }
        public string? Observaciones { get; private set; }

        public int Estado { get; private set; } = 1;
        public short UpdateToken { get; private set; }
        public int IdUsuarioCreador { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public int? IdUsuarioModificador { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

        public static Result<Sale> Create(
            EDocumento tipoDocumento,
            string numSerie,
            long correlativo,
            int idCliente,
            string nombreCliente,
            int idDocumentoIdentidad,
            string numDocumentoCliente,
            string direccionCliente,
            bool flagIGV,
            EFormaPago formaPago,
            ETipoMoneda tipoMoneda,
            decimal tipoCambio,
            decimal descuentoGlobal,
            int idEmpresa,
            int idSucursal,
            int idEstacion,
            int idTurnoAsistencia,
            int idTrabajador,
            int? idTrabajador2,
            int? idSubdiario,
            string? observaciones,
            int idUsuarioCreador,
            DateTime fechaCreacion)
        {
            if (correlativo <= 0)
                return Result.Failure<Sale>(SaleErrors.CorrelativoInvalido);

            if (string.IsNullOrWhiteSpace(numSerie))
                return Result.Failure<Sale>(SaleErrors.SerieRequerida);

            var sale = new Sale
            {
                IdEmpresa = idEmpresa,
                IdSucursal = idSucursal,
                IdEstacion = idEstacion,
                IdTurnoAsistencia = idTurnoAsistencia,
                FechaEmision = fechaCreacion,
                TipoDocumento = tipoDocumento,
                NumSerie = numSerie.Trim(),
                Correlativo = correlativo,
                IdCliente = idCliente,
                NombreCliente = nombreCliente?.Trim() ?? string.Empty,
                IdDocumentoIdentidad = idDocumentoIdentidad,
                NumDocumentoCliente = numDocumentoCliente?.Trim() ?? string.Empty,
                DireccionCliente = direccionCliente?.Trim() ?? string.Empty,
                FlagIGV = flagIGV,
                IdTrabajador = idTrabajador,
                IdTrabajador2 = idTrabajador2,
                FormaPago = formaPago,
                TipoMoneda = tipoMoneda,
                TipoCambio = tipoCambio <= 0 ? 1m : tipoCambio,
                DescuentoGlobal = descuentoGlobal,
                IdSubdiario = idSubdiario,
                Observaciones = observaciones,
                Estado = 1,
                UpdateToken = 1,
                IdUsuarioCreador = idUsuarioCreador,
                FechaCreacion = fechaCreacion
            };

            return Result.Success(sale);
        }

        public void AddItem(SaleItem item)
        {
            _items.Add(item);
        }

        public void SetTotals(
            decimal valorAfecto,
            decimal valorInafecto,
            decimal valorExonerado,
            decimal valorGratuito,
            decimal totalIsc,
            decimal igv,
            decimal totalIcbper,
            decimal importeTotal)
        {
            ValorAfecto = valorAfecto;
            ValorInafecto = valorInafecto;
            ValorExonerado = valorExonerado;
            ValorGratuito = valorGratuito;
            TotalIsc = totalIsc;
            Igv = igv;
            TotalIcbper = totalIcbper;
            ImporteTotal = importeTotal;
        }

        public Result Annul(int idUsuarioModificador, DateTime fechaModificacion)
        {
            if (Estado == 0)
                return Result.Failure(SaleErrors.Anulada(IdVenta));

            Estado = 0;
            IdUsuarioModificador = idUsuarioModificador;
            FechaModificacion = fechaModificacion;
            UpdateToken++;

            return Result.Success();
        }
    }
}
