using DataConsulting.PuntoVentaComercial.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class ClientLocalConfiguration : IEntityTypeConfiguration<ClientLocal>
    {
        public void Configure(EntityTypeBuilder<ClientLocal> builder)
        {
            builder.ToTable("LocalCliente", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IdLocal")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.IdCliente)
                .HasColumnName("IdCliente")
                .IsRequired();

            builder.Property(x => x.IdSucursal)
                .HasColumnName("IdSucursal")
                .IsRequired();

            builder.Property(x => x.DireccionLocal)
                .HasColumnName("DireccionLocal")
                .HasMaxLength(300)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Telefono1)
                .HasColumnName("Telefono1")
                .HasMaxLength(30)
                .IsUnicode(false);

            builder.Property(x => x.IdTipoCliente)
                .HasColumnName("IdTipoCliente")
                .IsRequired();

            builder.Property(x => x.Estado)
                .HasColumnName("Estado")
                .HasMaxLength(1)
                .IsRequired()
                .IsUnicode(false);
        }
    }
}
