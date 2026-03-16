using DataConsulting.PuntoVentaComercial.Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class SellerConfiguration : IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> builder)
        {
            builder.ToTable("Trabajador");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("IdTrabajador");
            builder.Property(x => x.IdEmpresa).IsRequired();
            builder.Property(x => x.Nombres).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Apellidos).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Codigo).HasMaxLength(20);
            builder.Property(x => x.PorcentajeDescuentoMaximo)
                .HasColumnName("PorcDescuentoMaximo")
                .HasColumnType("decimal(5,2)")
                .IsRequired();
            builder.Property(x => x.Activo).HasColumnName("Estado").IsRequired();
            builder.Ignore(x => x.NombreCompleto);
        }
    }
}
