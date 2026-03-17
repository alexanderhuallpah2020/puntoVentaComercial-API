using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

public sealed class Cliente : Entity
{
    public int IdTipoCliente { get; private set; }
    public string Nombre { get; private set; } = default!;
    public string? NombreComercial { get; private set; }
    public int? IdDocumentoIdentidad { get; private set; }
    public string? NumDocumento { get; private set; }
    public string? CodValidadorDoc { get; private set; }
    public string EstadoCliente { get; private set; } = default!;
    public string? Observaciones { get; private set; }
    public int FlagTipoCliente { get; private set; }
    public byte FlagSexo { get; private set; }
    public int FlagOperacion { get; private set; }
    public byte FlagCertCalidad { get; private set; }
    public short FlagCredito { get; private set; }
    public short FlagTipoCalificacion { get; private set; }
    public decimal CreditoMaximo { get; private set; }
    public int? IdTipoMoneda { get; private set; }
    public int? IdFormaPago { get; private set; }
    public int? IdGiroNegocio { get; private set; }
    public short? IdTrabajadorRef { get; private set; }
    public int? IdClienteRef { get; private set; }
    public int? IdEntidadRef { get; private set; }
    public short IdPais { get; private set; }
    public string? NombreOwner { get; private set; }
    public int? IdDocumentoIdentidadOwner { get; private set; }
    public string? NumDocumentoOwner { get; private set; }
    public DateTime? FechaNacimientoOwner { get; private set; }
    public DateTime FechaAlta { get; private set; }
    public DateTime? FechaBaja { get; private set; }
    public string UsuarioCreador { get; private set; } = default!;
    public DateTime FechaCreacion { get; private set; }
    public string? UsuarioModificador { get; private set; }
    public DateTime? FechaModificacion { get; private set; }

    private readonly List<ClienteLocal> _clienteLocales = [];
    public IReadOnlyCollection<ClienteLocal> ClienteLocales => _clienteLocales.AsReadOnly();

    private Cliente() { }

    public static Result<Cliente> Create(
        string nombre,
        int? idDocumentoIdentidad,
        string? numDocumento,
        string? codValidadorDoc,
        short idPais,
        string direccionLocal,
        string? telefono1,
        short idSucursal,
        int idLocal,
        string usuarioCreador)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Failure<Cliente>(ClienteErrors.NombreRequerido);

        if (string.IsNullOrWhiteSpace(direccionLocal))
            return Result.Failure<Cliente>(ClienteErrors.DireccionRequerida);

        if (idPais <= 0)
            return Result.Failure<Cliente>(ClienteErrors.PaisRequerido);

        var cliente = new Cliente
        {
            IdTipoCliente    = (int)ECliente.Varios,
            Nombre           = nombre.Trim(),
            NombreComercial  = nombre.Trim(),
            IdDocumentoIdentidad = idDocumentoIdentidad,
            NumDocumento     = numDocumento?.Trim(),
            CodValidadorDoc  = codValidadorDoc?.Trim(),
            EstadoCliente    = "A",
            FlagTipoCliente  = (int)ETipoCliente.ClienteLocal,
            FlagSexo         = (byte)EFlagSexo.Empresa,
            FlagOperacion    = 1,
            FlagCertCalidad  = 0,
            FlagCredito      = 0,
            FlagTipoCalificacion = 0,
            CreditoMaximo    = 0,
            IdPais           = idPais,
            FechaAlta        = DateTime.Now,
            UsuarioCreador   = usuarioCreador,
            FechaCreacion    = DateTime.Now
        };

        var local = ClienteLocal.Create(idLocal, direccionLocal, telefono1, idSucursal);
        cliente._clienteLocales.Add(local);

        return Result.Success(cliente);
    }

    public Result Update(
        string nombre,
        int? idDocumentoIdentidad,
        string? numDocumento,
        string? codValidadorDoc,
        short idPais,
        string direccionLocal,
        string? telefono1,
        string usuarioModificador)
    {
        if (IdTrabajadorRef.HasValue && IdTrabajadorRef > 0)
            return Result.Failure(ClienteErrors.NoEditableDesdePos);

        if (string.IsNullOrWhiteSpace(nombre))
            return Result.Failure(ClienteErrors.NombreRequerido);

        if (string.IsNullOrWhiteSpace(direccionLocal))
            return Result.Failure(ClienteErrors.DireccionRequerida);

        Nombre               = nombre.Trim();
        NombreComercial      = nombre.Trim();
        IdDocumentoIdentidad = idDocumentoIdentidad;
        NumDocumento         = numDocumento?.Trim();
        CodValidadorDoc      = codValidadorDoc?.Trim();
        IdPais               = idPais;
        UsuarioModificador   = usuarioModificador;
        FechaModificacion    = DateTime.Now;

        var local = _clienteLocales.FirstOrDefault();
        if (local is not null)
            local.Update(direccionLocal, telefono1);

        return Result.Success();
    }
}
