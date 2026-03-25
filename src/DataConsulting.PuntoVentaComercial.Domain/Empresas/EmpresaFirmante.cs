namespace DataConsulting.PuntoVentaComercial.Domain.Empresas;

public sealed record EmpresaFirmante(
    int Id,
    string RazonSocial,
    string NumDocumento,
    string UsuarioSunat,
    string ClaveSol,
    string NombreCertificado,
    string ClaveCertificado,
    string? Resolucion);
