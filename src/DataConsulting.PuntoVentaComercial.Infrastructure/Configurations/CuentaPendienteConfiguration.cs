using DataConsulting.PuntoVentaComercial.Domain.CuentasPendientes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class CuentaPendienteConfiguration : IEntityTypeConfiguration<CuentaPendiente>
{
    public void Configure(EntityTypeBuilder<CuentaPendiente> builder)
    {
        builder.ToTable("CuentaPendiente", "dbo");
        builder.HasKey(x => new { x.IdEmpresa, x.TipoOperacion, x.IdOperacion, x.Secuencia });

        builder.Property(x => x.IdEmpresa).HasColumnType("smallint").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.TipoOperacion).HasColumnType("tinyint").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.IdOperacion).HasColumnType("int").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.Secuencia).HasColumnType("smallint").ValueGeneratedNever().IsRequired();

        builder.Property(x => x.IdTipoMoneda).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.Importe).HasColumnType("money").IsRequired();
        builder.Property(x => x.Saldo).HasColumnType("money").IsRequired();
        builder.Property(x => x.FechaPago).HasColumnType("smalldatetime").IsRequired();
        builder.Property(x => x.Estado).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.IdEntidad).HasColumnType("int").IsRequired();
        builder.Property(x => x.Descuentos).HasColumnType("money").IsRequired();
        builder.Property(x => x.Retenciones).HasColumnType("money").IsRequired();
        builder.Property(x => x.SaldoRetencion).HasColumnType("money").IsRequired();
        builder.Property(x => x.UpdateToken).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.Detracciones).HasColumnType("money").IsRequired();
        builder.Property(x => x.SaldoDetraccion).HasColumnType("money").IsRequired();
        builder.Property(x => x.FlagTipo).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.MontoInteres).HasColumnType("money").IsRequired();
        builder.Property(x => x.Glosa).HasMaxLength(150).IsRequired();
        builder.Property(x => x.FlagFacturado).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.IdTipoDocumento).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.Percepciones).HasColumnType("money").IsRequired();
        builder.Property(x => x.SaldoPercepcion).HasColumnType("money").IsRequired();
        builder.Property(x => x.IdOperacionRef).HasColumnType("int");
        builder.Property(x => x.FlagInicial).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.FlagLiquidado).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.UsuarioCreador).HasColumnName("UsuarioCreador").HasMaxLength(20).IsRequired();
        builder.Property(x => x.FechaCreacion).HasColumnType("smalldatetime").IsRequired();

        // Shadow properties de auditoría
        builder.Property<string?>("UsuarioModificador").HasMaxLength(20);
        builder.Property<DateTime?>("FechaModificacion").HasColumnType("smalldatetime");
    }
}
