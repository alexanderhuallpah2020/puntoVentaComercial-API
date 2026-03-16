using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Clients
{
    public sealed class Client : Entity
    {
        private Client() { }

        public int IdCliente => Id;
        public string Nombre { get; private set; } = default!;
        public string? NombreComercial { get; private set; }
        public EDocumentoIdentidad IdDocumentoIdentidad { get; private set; }
        public string NumDocumento { get; private set; } = default!;
        public string? CodValidadorDoc { get; private set; }
        public int IdPais { get; private set; }
        public string Direccion { get; private set; } = default!;
        public bool FlagIGV { get; private set; }
        public decimal CreditoMaximo { get; private set; }
        public EEstado EstadoCliente { get; private set; }
        public DateTime FechaAlta { get; private set; }
        public DateTime? FechaBaja { get; private set; }
        public short IdUsuarioCreador { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public short? IdUsuarioModificador { get; private set; }
        public DateTime? FechaModificacion { get; private set; }

        public static Result<Client> Create(
            string nombre,
            string? nombreComercial,
            EDocumentoIdentidad idDocumentoIdentidad,
            string numDocumento,
            string? codValidadorDoc,
            int idPais,
            string direccion,
            bool flagIGV,
            decimal creditoMaximo,
            short idUsuarioCreador,
            DateTime fechaCreacion)
        {
            var client = new Client
            {
                Nombre = nombre.Trim().ToUpper(),
                NombreComercial = nombreComercial?.Trim().ToUpper(),
                IdDocumentoIdentidad = idDocumentoIdentidad,
                NumDocumento = numDocumento.Trim(),
                CodValidadorDoc = codValidadorDoc?.Trim().ToUpper(),
                IdPais = idPais,
                Direccion = direccion.Trim().ToUpper(),
                FlagIGV = flagIGV,
                CreditoMaximo = creditoMaximo,
                EstadoCliente = EEstado.Activo,
                FechaAlta = fechaCreacion,
                IdUsuarioCreador = idUsuarioCreador,
                FechaCreacion = fechaCreacion
            };

            return Result.Success(client);
        }

        public Result Update(
            string nombre,
            string? nombreComercial,
            string direccion,
            bool flagIGV,
            decimal creditoMaximo,
            short idUsuarioModificador,
            DateTime fechaModificacion)
        {
            Nombre = nombre.Trim().ToUpper();
            NombreComercial = nombreComercial?.Trim().ToUpper();
            Direccion = direccion.Trim().ToUpper();
            FlagIGV = flagIGV;
            CreditoMaximo = creditoMaximo;
            IdUsuarioModificador = idUsuarioModificador;
            FechaModificacion = fechaModificacion;

            return Result.Success();
        }
    }
}
