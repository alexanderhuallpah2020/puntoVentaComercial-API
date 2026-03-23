using DataConsulting.PuntoVentaComercial.Domain.Ventas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class VentaEmisionConfiguration : IEntityTypeConfiguration<VentaEmision>
{
    public void Configure(EntityTypeBuilder<VentaEmision> builder)
    {
        builder.ToTable("VentaEmision", "dbo");
        builder.HasKey(x => x.IdVenta);
        // IdVenta es FK de Venta — no IDENTITY
        builder.Property(x => x.IdVenta).ValueGeneratedNever().IsRequired();

        builder.Property(x => x.ClienteNombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ClienteDireccion).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ClienteDocumento).HasMaxLength(20);
        builder.Property(x => x.Observacion).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.PuntosBonus).HasColumnType("int").IsRequired();
        builder.Property(x => x.Referencias).HasMaxLength(200);
        builder.Property(x => x.ClienteCodValidadorDoc).HasMaxLength(3);
        builder.Property(x => x.IdUsuarioCreador).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.FechaCreacion).HasColumnType("smalldatetime").IsRequired();

        // Shadow properties de auditoría
        builder.Property<short?>("IdUsuarioModificador").HasColumnType("smallint");
        builder.Property<DateTime?>("FechaModificacion").HasColumnType("smalldatetime");
    }
}
