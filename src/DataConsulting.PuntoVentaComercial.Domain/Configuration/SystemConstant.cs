namespace DataConsulting.PuntoVentaComercial.Domain.Configuration
{
    /// <summary>
    /// Proyección plana de constantes del sistema por empresa/sucursal.
    /// No es una entidad EF — se carga como resultado de query Dapper.
    /// </summary>
    public sealed record SystemConstant(
        string Clave,
        string Valor,
        string? Descripcion
    );
}
