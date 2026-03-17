using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Cash.Commands.CreateVaultDeposit
{
    public sealed record CreateVaultDepositCommand(
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
        int IdUsuarioCreador
    ) : ICommand<CreateVaultDepositResponse>;
}
