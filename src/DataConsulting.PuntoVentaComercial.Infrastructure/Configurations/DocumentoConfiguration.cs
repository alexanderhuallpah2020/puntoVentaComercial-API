using DataConsulting.PuntoVentaComercial.Domain.Documentos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

internal sealed class DocumentoConfiguration : IEntityTypeConfiguration<Documento>
{
    public void Configure(EntityTypeBuilder<Documento> builder)
    {
        builder.ToTable("Documento");

        builder.HasKey(x => x.IdTipoDocumento);
        builder.Property(x => x.IdTipoDocumento).ValueGeneratedNever();

        builder.Property(x => x.Siglas).HasMaxLength(5).IsUnicode(false);
        builder.Property(x => x.Descripcion).HasMaxLength(100).IsUnicode(false);
        builder.Property(x => x.CodigoSunat).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.FormatoNumero).HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(x => x.SiglaSEE).HasMaxLength(3).IsUnicode(false).IsRequired();
        builder.Property(x => x.CodCONCAR).HasColumnType("char(2)").IsUnicode(false);

        builder.Property(x => x.DocumentoFlags).HasComputedColumnSql(null).ValueGeneratedOnAddOrUpdate();
    }
}
