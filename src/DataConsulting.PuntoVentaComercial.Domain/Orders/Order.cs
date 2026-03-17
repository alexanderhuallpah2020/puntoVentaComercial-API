using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Orders
{
    public sealed class Order : Entity
    {
        private readonly List<OrderItem> _items = [];

        // ORM constructor
        private Order() { }

        public int IdPedido => Id;
        public int IdEmpresa { get; private set; }
        public int IdSucursal { get; private set; }
        public int IdEstacion { get; private set; }
        public int IdTurnoAsistencia { get; private set; }
        public DateTime FechaEmision { get; private set; }
        public EDocumento TipoDocumento { get; private set; }
        public string NumSerie { get; private set; } = string.Empty;
        public long Correlativo { get; private set; }

        // Cliente denormalizado
        public int IdCliente { get; private set; }
        public string NombreCliente { get; private set; } = string.Empty;
        public int IdDocumentoIdentidad { get; private set; }
        public string NumDocumentoCliente { get; private set; } = string.Empty;
        public string DireccionCliente { get; private set; } = string.Empty;
        public bool FlagIGV { get; private set; }

        public int IdTrabajador { get; private set; }
        public int? IdTrabajador2 { get; private set; }
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

        public string? Observaciones { get; private set; }

        // 1=Pendiente, 4=Anulado (EEstadoPedido)
        public int Estado { get; private set; } = 1;
        public short UpdateToken { get; private set; }
        public int IdUsuarioCreador { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public int? IdUsuarioModificador { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public static Result<Order> Create(
            EDocumento tipoDocumento,
            string numSerie,
            long correlativo,
            int idCliente,
            string nombreCliente,
            int idDocumentoIdentidad,
            string numDocumentoCliente,
            string direccionCliente,
            bool flagIGV,
            ETipoMoneda tipoMoneda,
            decimal tipoCambio,
            decimal descuentoGlobal,
            int idEmpresa,
            int idSucursal,
            int idEstacion,
            int idTurnoAsistencia,
            int idTrabajador,
            int? idTrabajador2,
            string? observaciones,
            int idUsuarioCreador,
            DateTime fechaCreacion)
        {
            if (correlativo <= 0)
                return Result.Failure<Order>(OrderErrors.CorrelativoInvalido);

            if (string.IsNullOrWhiteSpace(numSerie))
                return Result.Failure<Order>(OrderErrors.SerieRequerida);

            return Result.Success(new Order
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
                TipoMoneda = tipoMoneda,
                TipoCambio = tipoCambio <= 0 ? 1m : tipoCambio,
                DescuentoGlobal = descuentoGlobal,
                Observaciones = observaciones,
                Estado = (int)EEstadoPedido.Pendiente,
                UpdateToken = 1,
                IdUsuarioCreador = idUsuarioCreador,
                FechaCreacion = fechaCreacion
            });
        }

        public void AddItem(OrderItem item) => _items.Add(item);

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
            if (Estado == (int)EEstadoPedido.Anulado)
                return Result.Failure(OrderErrors.Anulado(IdPedido));

            Estado = (int)EEstadoPedido.Anulado;
            IdUsuarioModificador = idUsuarioModificador;
            FechaModificacion = fechaModificacion;
            UpdateToken++;

            return Result.Success();
        }
    }
}
