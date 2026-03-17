using DataConsulting.PuntoVentaComercial.Domain.Paises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class PaisConfiguration : IEntityTypeConfiguration<Pais>
{
    public void Configure(EntityTypeBuilder<Pais> builder)
    {
        builder.ToTable("Paises", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("idPais").HasColumnType("smallint").ValueGeneratedNever();

        builder.Property(x => x.Nombre).HasMaxLength(40);
        builder.Property(x => x.Estado).HasMaxLength(1).IsFixedLength();
        builder.Property(x => x.CodSunat).HasMaxLength(4);
        builder.Property(x => x.CodNacionalidad).HasMaxLength(4);
    }
}
