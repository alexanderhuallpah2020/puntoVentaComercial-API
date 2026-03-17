using DataConsulting.PuntoVentaComercial.Domain.Sunat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class SunatSubmissionConfiguration : IEntityTypeConfiguration<SunatSubmission>
    {
        public void Configure(EntityTypeBuilder<SunatSubmission> builder)
        {
            builder.ToTable("SunatEnvio", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IdEnvio")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.IdVenta).HasColumnName("IdVenta").IsRequired();

            // Índice único: una venta tiene como máximo un registro de envío
            builder.HasIndex(x => x.IdVenta).IsUnique();

            builder.Property(x => x.FechaEnvio)
                .HasColumnName("FechaEnvio")
                .HasColumnType("smalldatetime")
                .IsRequired();

            builder.Property(x => x.Estado).HasColumnName("Estado").IsRequired();

            builder.Property(x => x.CodigoRespuesta)
                .HasColumnName("CodigoRespuesta")
                .HasMaxLength(10)
                .IsUnicode(false);

            builder.Property(x => x.MensajeRespuesta)
                .HasColumnName("MensajeRespuesta")
                .HasMaxLength(500)
                .IsUnicode(false);

            builder.Property(x => x.XmlHash)
                .HasColumnName("XmlHash")
                .HasMaxLength(64)  // SHA-256 hex = 64 chars
                .IsUnicode(false);

            builder.Property(x => x.CdrXml)
                .HasColumnName("CdrXml")
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.NumTicket)
                .HasColumnName("NumTicket")
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(x => x.Intentos).HasColumnName("Intentos").IsRequired();

            builder.Property(x => x.FechaCreacion)
                .HasColumnName("FechaCreacion")
                .HasColumnType("smalldatetime")
                .IsRequired();

            builder.Property(x => x.FechaModificacion)
                .HasColumnName("FechaModificacion")
                .HasColumnType("smalldatetime");
        }
    }
}
