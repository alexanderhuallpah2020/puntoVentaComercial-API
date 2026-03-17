using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Cash
{
    public sealed record CreateVaultDepositRequest(
        int IdEmpresa,
        int IdSucursal,
        int IdTrabajador,
        int IdIsla,
        int IdTurnoAsistencia,
        EDocumento TipoDocumento,
        string NumSerie,
        string NumDocumento,
        ETipoMoneda TipoMoneda,
        decimal TipoCambio,
        decimal Importe,
        int IdFormaPago,
        string? Glosa,
        int IdUsuarioCreador);
}
