using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Domain.Clientes;

public sealed class ClienteLocal : Entity
{
    public int IdCliente { get; private set; }
    public short IdSucursal { get; private set; }
    public string DireccionLocal { get; private set; } = default!;
    public string? Telefono1 { get; private set; }
    public int IdTipoCliente { get; private set; }
    public string Estado { get; private set; } = default!;
    public int IdLocalUnico { get; private set; }

    private ClienteLocal() { }

    internal static ClienteLocal Create(
        int id,
        int idLocalUnico,
        string direccionLocal,
        string? telefono1,
        short idSucursal) => new()
        {
            Id = id,
            IdLocalUnico = idLocalUnico,
            DireccionLocal = direccionLocal.Trim(),
            Telefono1 = telefono1?.Trim(),
            IdSucursal = idSucursal,
            IdTipoCliente = (int)ECliente.Varios,
            Estado = "A"
        };

    internal void Update(string direccionLocal, string? telefono1)
    {
        DireccionLocal = direccionLocal.Trim();
        Telefono1 = telefono1?.Trim();
    }
}
