using DataConsulting.PuntoVentaComercial.Domain.OperacionesPago;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class OperacionPagoDetalleConfiguration : IEntityTypeConfiguration<OperacionPagoDetalle>
{
    public void Configure(EntityTypeBuilder<OperacionPagoDetalle> builder)
    {
        builder.ToTable("OperacionPagoDetalle", "dbo");
        builder.HasKey(x => new { x.IdEmpresa, x.TipoOperacion, x.NroOperacion, x.Secuencia });

        builder.Property(x => x.IdEmpresa).HasColumnType("smallint").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.TipoOperacion).HasColumnType("tinyint").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.NroOperacion).HasColumnType("int").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Secuencia).HasColumnType("smallint").ValueGeneratedNever().IsRequired();

        builder.Property(x => x.IdFormaPago).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdTipoMoneda).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.Importe).HasColumnType("money").IsRequired();
        builder.Property(x => x.IdDocumentoRef).HasColumnType("int");
        builder.Property(x => x.SecuenciaRef).HasColumnType("int");
        builder.Property(x => x.Estado).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.NumReferencia).HasMaxLength(30).IsRequired();
        builder.Property(x => x.SecuenciaEntidadRef).HasColumnType("smallint");
        builder.Property(x => x.IdOperacionPagoTributo).HasColumnType("int");
    }
}
