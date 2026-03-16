using DataConsulting.PuntoVentaComercial.Domain.FamiliasSunat;
using DataConsulting.PuntoVentaComercial.Domain.SegmentosSunat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class FamiliaSunatConfiguration : IEntityTypeConfiguration<FamiliaSunat>
    {
        public void Configure(EntityTypeBuilder<FamiliaSunat> builder)
        {
            builder.ToTable("FamiliaSunat", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IdFamiliaSunat")
                .ValueGeneratedNever();

            builder.Property(x => x.IdSegmentoSunat)
                .HasColumnName("IdSegmentoSunat")
                .IsRequired();

            builder.Property(x => x.Codigo)
                .HasColumnName("Codigo")
                .HasMaxLength(10)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Descripcion)
                .HasColumnName("Descripcion")
                .HasMaxLength(200)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Estado)
                .HasColumnName("Estado")
                .HasColumnType("smallint")
                .IsRequired()
                .HasConversion<short>();

            builder.Property(x => x.UpdateToken)
                .HasColumnName("UpdateToken")
                .HasColumnType("smallint")
                .IsRequired()
                .IsConcurrencyToken();

            builder.Property(x => x.IdUsuarioCreador)
                .HasColumnName("IdUsuarioCreador")
                .HasColumnType("smallint")
                .IsRequired();

            builder.Property(x => x.FechaCreacion)
                .HasColumnName("FechaCreacion")
                .HasColumnType("smalldatetime")
                .IsRequired();

            builder.Property(x => x.IdUsuarioModificador)
                .HasColumnName("IdUsuarioModificador")
                .HasColumnType("smallint");

            builder.Property(x => x.FechaModificacion)
                .HasColumnName("FechaModificacion")
                .HasColumnType("smalldatetime");

            builder.HasOne<SegmentoSunat>()
                .WithMany()
                .HasForeignKey(x => x.IdSegmentoSunat)
                .HasConstraintName("FK_FamiliaSunat_SegmentoSunat");
        }
    }
}
