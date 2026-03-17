using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    public sealed class Client : Entity
    {
        private readonly List<ClientLocal> _locals = [];

        // ORM constructor
        private Client() { }

        public int IdCliente => Id;
        public string Nombre { get; private set; } = string.Empty;
        public string NombreComercial { get; private set; } = string.Empty;
        public EDocumentoIdentidad IdDocumentoIdentidad { get; private set; }
        public string NumDocumento { get; private set; } = string.Empty;
        // Dígito validador del DNI peruano (1 carácter: 0-9 o K)
        public string CodValidadorDoc { get; private set; } = string.Empty;
        public int IdPais { get; private set; }
        public int IdTipoCliente { get; private set; }
        public bool FlagIGV { get; private set; }
        public decimal CreditoMaximo { get; private set; }
        public string EstadoCliente { get; private set; } = "A";
        public DateTime FechaAlta { get; private set; }
        public DateTime? FechaBaja { get; private set; }
        public short UpdateToken { get; private set; }
        public int IdUsuarioCreador { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public int? IdUsuarioModificador { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        public IReadOnlyCollection<ClientLocal> Locals => _locals.AsReadOnly();

        /// <summary>
        /// Crea un nuevo cliente validando reglas básicas de negocio.
        /// No valida el check digit DNI/RUC — esa validación se hace en Application antes de llamar Create.
        /// </summary>
        public static Result<Client> Create(
            string nombre,
            EDocumentoIdentidad idDocumentoIdentidad,
            string numDocumento,
            string codValidadorDoc,
            int idPais,
            int idTipoCliente,
            int idUsuarioCreador,
            DateTime fechaCreacion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Result.Failure<Client>(ClientErrors.NombreRequerido);

            if (idPais <= 0)
                return Result.Failure<Client>(ClientErrors.PaisRequerido);

            var client = new Client
            {
                Nombre = nombre.Trim(),
                NombreComercial = nombre.Trim(),
                IdDocumentoIdentidad = idDocumentoIdentidad,
                NumDocumento = numDocumento?.Trim() ?? string.Empty,
                CodValidadorDoc = codValidadorDoc?.Trim() ?? string.Empty,
                IdPais = idPais,
                IdTipoCliente = idTipoCliente,
                FlagIGV = idDocumentoIdentidad == EDocumentoIdentidad.RUC,
                CreditoMaximo = 0,
                EstadoCliente = "A",
                FechaAlta = fechaCreacion,
                UpdateToken = 1,
                IdUsuarioCreador = idUsuarioCreador,
                FechaCreacion = fechaCreacion
            };

            return Result.Success(client);
        }

        /// <summary>
        /// Actualiza los datos del cliente. Preserva IdDocumentoIdentidad/NumDocumento solo si no cambian
        /// (en el legacy solo Nombre, Dirección y Teléfono se editan desde el POS).
        /// </summary>
        public Result Update(
            string nombre,
            string codValidadorDoc,
            int idUsuarioModificador,
            DateTime fechaModificacion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return Result.Failure(ClientErrors.NombreRequerido);

            Nombre = nombre.Trim();
            NombreComercial = nombre.Trim();
            CodValidadorDoc = codValidadorDoc?.Trim() ?? string.Empty;
            IdUsuarioModificador = idUsuarioModificador;
            FechaModificacion = fechaModificacion;
            UpdateToken++;

            return Result.Success();
        }

        public void AddLocal(ClientLocal local)
        {
            _locals.Add(local);
        }

        public void UpdateLocal(string direccion, string telefono)
        {
            var local = _locals.FirstOrDefault();
            if (local is not null)
                local.Update(direccion, telefono);
        }
    }
}
