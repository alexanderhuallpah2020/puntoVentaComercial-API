using DataConsulting.PuntoVentaComercial.Domain.SegmentosSunat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

internal sealed class SegmentoSunatConfiguration : IEntityTypeConfiguration<SegmentoSunat>
{
    public void Configure(EntityTypeBuilder<SegmentoSunat> builder)
    {
        builder.ToTable("SegmentoSunat", "dbo");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("IdSegmentoSunat")
            .ValueGeneratedNever();

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
            // Si EEstado es int, esto lo guarda como smallint en BD.
            .HasConversion<short>();

        builder.Property(x => x.UpdateToken)
            .HasColumnName("UpdateToken")
            .HasColumnType("smallint")
            .IsRequired();

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

        // Si UpdateToken funciona como token de concurrencia (no rowversion),
        // use concurrency token.
        builder.Property(x => x.UpdateToken)
            .IsConcurrencyToken();
    }
}