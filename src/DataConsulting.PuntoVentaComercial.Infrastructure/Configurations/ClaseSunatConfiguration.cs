using DataConsulting.PuntoVentaComercial.Domain.ClasesSunat;
using DataConsulting.PuntoVentaComercial.Domain.FamiliasSunat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class ClaseSunatConfiguration : IEntityTypeConfiguration<ClaseSunat>
    {
        public void Configure(EntityTypeBuilder<ClaseSunat> builder)
        {
            builder.ToTable("ClaseSunat", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IdClaseSunat")
                .ValueGeneratedNever();

            builder.Property(x => x.IdFamiliaSunat)
                .HasColumnName("IdFamiliaSunat")
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

            builder.HasOne<FamiliaSunat>()
                .WithMany()
                .HasForeignKey(x => x.IdFamiliaSunat)
                .HasConstraintName("FK_ClaseSunat_FamiliaSunat");
        }
    }
}
