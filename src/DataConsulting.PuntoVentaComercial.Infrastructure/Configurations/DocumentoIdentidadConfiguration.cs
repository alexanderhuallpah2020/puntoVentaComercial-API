using DataConsulting.PuntoVentaComercial.Domain.DocumentosIdentidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class DocumentoIdentidadConfiguration : IEntityTypeConfiguration<DocumentoIdentidad>
{
    public void Configure(EntityTypeBuilder<DocumentoIdentidad> builder)
    {
        builder.ToTable("DocumentoIdentidad", "dbo");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("IdDocumentoIdentidad").HasColumnType("smallint").ValueGeneratedNever();

        builder.Property(x => x.Nombre).HasColumnName("DocumentoIdentidad").HasMaxLength(40);
        builder.Property(x => x.Longitud).IsRequired();
        builder.Property(x => x.OrdenEfecto).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.ValorEntero).HasColumnType("tinyint");
        builder.Property(x => x.IdPais).HasColumnType("smallint");
        builder.Property(x => x.MaskDocumento).HasMaxLength(20);
        builder.Property(x => x.Estado).HasColumnType("tinyint");
        builder.Property(x => x.CodConcar).HasColumnName("CodCONCar").HasMaxLength(2).IsFixedLength();
        builder.Property(x => x.CodSunat).HasMaxLength(2).IsFixedLength();
    }
}
