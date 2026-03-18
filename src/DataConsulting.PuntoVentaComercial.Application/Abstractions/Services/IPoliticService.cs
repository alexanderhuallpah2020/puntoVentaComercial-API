using DataConsulting.PuntoVentaComercial.Domain.Enums;

namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Services;

public interface IPoliticService
{
    /// <summary>
    /// Retorna true si el usuario tiene la política otorgada en Security.GetGrantedPolitic.
    /// </summary>
    Task<bool> HasPoliticAsync(string userName, EPolitica politic, CancellationToken cancellationToken);
}
