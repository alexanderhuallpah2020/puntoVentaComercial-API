using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class ClienteLocalConfiguration : IEntityTypeConfiguration<ClienteLocal>
{
    public void Configure(EntityTypeBuilder<ClienteLocal> builder)
    {
        builder.ToTable("LocalCliente", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("IdLocal").ValueGeneratedNever();

        builder.Property(x => x.IdCliente).IsRequired();
        builder.Property(x => x.IdLocalUnico).IsRequired();
        builder.Property(x => x.IdTipoCliente).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdSucursal).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.DireccionLocal).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Telefono1).HasMaxLength(30);
        builder.Property(x => x.Estado).HasMaxLength(1).IsFixedLength().IsRequired();

        // Columnas NOT NULL sin default en BD — defaults aplicados en SaveChangesAsync del DbContext
        builder.Property<string>("ReferenciaDireccion").HasMaxLength(100).IsRequired().HasDefaultValue("");
        builder.Property<string>("Faxlocal").HasMaxLength(15).IsRequired().HasDefaultValue("");
        builder.Property<string>("CargoContacto").HasMaxLength(60).IsRequired().HasDefaultValue("");
        builder.Property<string>("EMail").HasMaxLength(60).IsRequired().HasDefaultValue("");
        builder.Property<string>("CodigoMapInfo").HasMaxLength(10).IsRequired().HasDefaultValue("");
        builder.Property<string>("Celular1").HasMaxLength(15).IsRequired().HasDefaultValue("");
        builder.Property<string>("CodigoLocal").HasMaxLength(10).IsRequired().HasDefaultValue("");
    }
}
