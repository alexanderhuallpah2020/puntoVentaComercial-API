namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetConstants
{
    public sealed record GetConstantsResponse(IReadOnlyList<SystemConstantDto> Constants);

    public sealed record SystemConstantDto(
        string Clave,
        string Valor,
        string? Descripcion
    );
}
