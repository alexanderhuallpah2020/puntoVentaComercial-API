using DataConsulting.PuntoVentaComercial.Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class DocumentSeriesConfiguration : IEntityTypeConfiguration<DocumentSeries>
    {
        public void Configure(EntityTypeBuilder<DocumentSeries> builder)
        {
            builder.ToTable("DocumentoSerie");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("IdDocumentoSerie");
            builder.Property(x => x.IdEmpresa).IsRequired();
            builder.Property(x => x.IdSucursal).IsRequired();
            builder.Property(x => x.IdEstacion).IsRequired();
            builder.Property(x => x.TipoDocumento).HasColumnName("IdTipoDocumento").IsRequired();
            builder.Property(x => x.NumSerie).HasMaxLength(10).IsRequired();
            builder.Property(x => x.UltimoCorrelativo).IsRequired();
            builder.Property(x => x.Activo).HasColumnName("Estado").IsRequired();
        }
    }
}
