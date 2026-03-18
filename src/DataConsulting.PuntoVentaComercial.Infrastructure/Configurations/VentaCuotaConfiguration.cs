using DataConsulting.PuntoVentaComercial.Domain.Ventas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class VentaCuotaConfiguration : IEntityTypeConfiguration<VentaCuota>
{
    public void Configure(EntityTypeBuilder<VentaCuota> builder)
    {
        builder.ToTable("CuotasVenta", "dbo");
        builder.HasKey(x => new { x.IdVenta, x.Correlativo });

        builder.Property(x => x.IdVenta).IsRequired();
        builder.Property(x => x.Correlativo).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.FechaCuota).HasColumnType("datetime");     // DATETIME NULL
        builder.Property(x => x.Monto).HasColumnType("decimal(8,2)");      // DECIMAL(8,2) NULL
        builder.Property(x => x.NumeroCuota).HasMaxLength(8);              // VARCHAR(8) NULL
        builder.Property(x => x.Glosa).HasColumnType("decimal(8,2)");      // DECIMAL(8,2) NULL
    }
}
