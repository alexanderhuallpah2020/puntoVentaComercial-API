namespace DataConsulting.PuntoVentaComercial.Domain.Ventas;

public sealed class VentaEmision
{
    public int IdVenta { get; private set; }
    public string ClienteNombre { get; private set; } = default!;
    public string ClienteDireccion { get; private set; } = default!;
    public string? ClienteDocumento { get; private set; }
    public string Observacion { get; private set; } = default!;
    public int PuntosBonus { get; private set; }
    public string? Referencias { get; private set; }
    public string? ClienteCodValidadorDoc { get; private set; }
    public short IdUsuarioCreador { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    private VentaEmision() { }

    public static VentaEmision Create(
        string clienteNombre,
        string clienteDireccion,
        string? clienteDocumento,
        string observacion,
        int puntosBonus,
        string? referencias,
        string? clienteCodValidadorDoc,
        short idUsuarioCreador = 1)
    {
        return new VentaEmision
        {
            ClienteNombre           = clienteNombre,
            ClienteDireccion        = clienteDireccion,
            ClienteDocumento        = clienteDocumento,
            Observacion             = observacion,
            PuntosBonus             = puntosBonus,
            Referencias             = referencias,
            ClienteCodValidadorDoc  = clienteCodValidadorDoc,
            IdUsuarioCreador        = idUsuarioCreador,
            FechaCreacion           = DateTime.Now
        };
    }
}
