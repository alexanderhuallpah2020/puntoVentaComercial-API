namespace DataConsulting.PuntoVentaComercial.Domain.OperacionesPago;

public sealed class OperacionPagoDetalle
{
    public short IdEmpresa { get; private set; }
    public byte TipoOperacion { get; private set; }
    public int NroOperacion { get; private set; }
    public short Secuencia { get; private set; }
    public short IdFormaPago { get; private set; }
    public short IdTipoMoneda { get; private set; }
    public decimal Importe { get; private set; }
    public int? IdDocumentoRef { get; private set; }
    public int? SecuenciaRef { get; private set; }
    public byte Estado { get; private set; }
    public string NumReferencia { get; private set; } = default!;
    public short? SecuenciaEntidadRef { get; private set; }
    public int? IdOperacionPagoTributo { get; private set; }

    private OperacionPagoDetalle() { }

    public static OperacionPagoDetalle Create(
        short idEmpresa,
        byte tipoOperacion,
        int nroOperacion,
        short secuencia,
        short idFormaPago,
        short idTipoMoneda,
        decimal importe,
        string numReferencia = "")
    {
        return new OperacionPagoDetalle
        {
            IdEmpresa              = idEmpresa,
            TipoOperacion          = tipoOperacion,
            NroOperacion           = nroOperacion,
            Secuencia              = secuencia,
            IdFormaPago            = idFormaPago,
            IdTipoMoneda           = idTipoMoneda,
            Importe                = importe,
            IdDocumentoRef         = null,
            SecuenciaRef           = null,
            Estado                 = 1,
            NumReferencia          = numReferencia,
            SecuenciaEntidadRef    = null,
            IdOperacionPagoTributo = null
        };
    }
}
