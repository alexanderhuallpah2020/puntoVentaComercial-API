using DataConsulting.PuntoVentaComercial.Domain.Ventas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class VentaPagoConfiguration : IEntityTypeConfiguration<VentaPago>
{
    public void Configure(EntityTypeBuilder<VentaPago> builder)
    {
        builder.ToTable("VentaFormaPago", "dbo");
        builder.HasKey(x => new { x.IdVenta, x.IdFormaPago });

        builder.Property(x => x.IdVenta).IsRequired();
        builder.Property(x => x.IdFormaPago).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdTipoMoneda).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.Importe).HasColumnType("smallmoney").IsRequired();
        builder.Property(x => x.Estado).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.UpdateToken).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdUsuarioCreador).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.FechaCreacion).HasColumnType("smalldatetime").IsRequired();

        // Shadow properties para columnas nullable de auditoría
        builder.Property<short?>("IdUsuarioModificacion").HasColumnType("smallint");
        builder.Property<DateTime?>("FechaModificacion").HasColumnType("smalldatetime");
    }
}
