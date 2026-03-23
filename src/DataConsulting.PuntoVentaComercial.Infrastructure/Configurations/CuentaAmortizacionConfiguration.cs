using DataConsulting.PuntoVentaComercial.Domain.OperacionesPago;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class CuentaAmortizacionConfiguration : IEntityTypeConfiguration<CuentaAmortizacion>
{
    public void Configure(EntityTypeBuilder<CuentaAmortizacion> builder)
    {
        builder.ToTable("CuentaAmortizacion", "dbo");
        builder.HasKey(x => new
        {
            x.IdEmpresa,
            x.TipoOperacion,
            x.NroOperacion,
            x.TipoOperacionRef,
            x.IdOperacion,
            x.Secuencia
        });

        builder.Property(x => x.IdEmpresa).HasColumnType("smallint").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.TipoOperacion).HasColumnType("tinyint").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.NroOperacion).HasColumnType("int").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.TipoOperacionRef).HasColumnType("tinyint").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.IdOperacion).HasColumnType("int").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Secuencia).HasColumnType("smallint").ValueGeneratedNever().IsRequired();

        builder.Property(x => x.Importe).HasColumnType("money").IsRequired();
        builder.Property(x => x.Estado).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.Retencion).HasColumnType("money").IsRequired();
        builder.Property(x => x.Descuento).HasColumnType("money").IsRequired();
        builder.Property(x => x.Detraccion).HasColumnType("money").IsRequired();
        builder.Property(x => x.Percepcion).HasColumnType("money").IsRequired();
    }
}
