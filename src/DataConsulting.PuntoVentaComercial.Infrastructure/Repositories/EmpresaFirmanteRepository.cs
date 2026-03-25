using DataConsulting.PuntoVentaComercial.Domain.Empresas;
using DataConsulting.PuntoVentaComercial.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Repositories;

internal sealed class EmpresaFirmanteRepository(ApplicationDbContext dbContext)
    : IEmpresaFirmanteRepository
{
    public async Task<EmpresaFirmante?> GetByIdAsync(int idEmpresa, CancellationToken ct)
    {
        var results = await dbContext.Database
            .SqlQuery<EmpresaFirmanteRow>($"EXEC dbo.GetEmpresaFirmante {idEmpresa}")
            .ToListAsync(ct);

        var row = results.FirstOrDefault();
        if (row is null) return null;

        return new EmpresaFirmante(
            row.IdEmpresaFirmante,
            row.RazonSocial,
            row.NumDocumento,
            row.UsuarioSunat,
            row.ClaveSol,
            row.NombreCertificado,
            row.ClaveCertificado,
            row.Resolucion);
    }

    // Projection type for SqlQuery — must be a class with a parameterless constructor
    private sealed class EmpresaFirmanteRow
    {
        public int IdEmpresaFirmante { get; set; }
        public string RazonSocial { get; set; } = default!;
        public string NumDocumento { get; set; } = default!;
        public string UsuarioSunat { get; set; } = default!;
        public string ClaveSol { get; set; } = default!;
        public string NombreCertificado { get; set; } = default!;
        public string ClaveCertificado { get; set; } = default!;
        public string? Resolucion { get; set; }
    }
}
