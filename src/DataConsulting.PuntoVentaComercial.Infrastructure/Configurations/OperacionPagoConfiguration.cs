using DataConsulting.PuntoVentaComercial.Domain.OperacionesPago;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class OperacionPagoConfiguration : IEntityTypeConfiguration<OperacionPago>
{
    public void Configure(EntityTypeBuilder<OperacionPago> builder)
    {
        builder.ToTable("OperacionPago", "dbo");
        builder.HasKey(x => new { x.IdEmpresa, x.TipoOperacion, x.NroOperacion });

        builder.Property(x => x.IdEmpresa).HasColumnType("smallint").ValueGeneratedNever().IsRequired();
        builder.Property(x => x.TipoOperacion).HasColumnType("tinyint").ValueGeneratedNever().IsRequired();
        // NroOperacion se genera con MAX+1 antes de insertar
        builder.Property(x => x.NroOperacion).HasColumnType("int").ValueGeneratedNever().IsRequired();

        builder.Property(x => x.FechaEmision).HasColumnType("smalldatetime").IsRequired();
        builder.Property(x => x.IdTipoMoneda).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.ImporteTotal).HasColumnType("money").IsRequired();
        builder.Property(x => x.ImportePago).HasColumnType("money").IsRequired();
        builder.Property(x => x.IdSucursal).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdTrabajador).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdEstacion).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.Estado).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.TipoCambio).HasColumnType("money").IsRequired();
        builder.Property(x => x.Retenciones).HasColumnType("money").IsRequired();
        builder.Property(x => x.Descuentos).HasColumnType("money").IsRequired();
        builder.Property(x => x.IdEntidad).HasColumnType("int").IsRequired();
        builder.Property(x => x.Observaciones).HasMaxLength(200).IsRequired();
        builder.Property(x => x.UpdateToken).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.IdTurnoAsistencia).HasColumnType("smallint");
        builder.Property(x => x.IdDocumentoRef).HasColumnType("int").IsRequired();
        builder.Property(x => x.Detracciones).HasColumnType("money").IsRequired();
        builder.Property(x => x.Percepciones).HasColumnType("money").IsRequired();
        builder.Property(x => x.IdTipoDocumentoRef).HasColumnType("smallint");
        builder.Property(x => x.IdConceptoPago).HasColumnType("int");
        builder.Property(x => x.NumSerieDocumentoRef).HasMaxLength(30).IsRequired();
        builder.Property(x => x.IdLiquidacion).HasColumnType("int");
        builder.Property(x => x.IdCajaChica).HasColumnType("int");
        builder.Property(x => x.EstadoContable).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.FlagRendido).HasColumnType("tinyint");
        builder.Property(x => x.IdRendicionCajaChica).HasColumnType("int");
        builder.Property(x => x.IdConceptoCtaCte).HasColumnType("smallint");
        builder.Property(x => x.UsuarioInsert).HasMaxLength(20).IsRequired();
        builder.Property(x => x.FechaInsert).HasColumnType("smalldatetime").IsRequired();

        // Shadow properties de auditoría
        builder.Property<string?>("UsuarioUpdate").HasMaxLength(20);
        builder.Property<DateTime?>("FechaUpdate").HasColumnType("smalldatetime");

        builder.HasMany(x => x.Detalles)
               .WithOne()
               .HasForeignKey(x => new { x.IdEmpresa, x.TipoOperacion, x.NroOperacion })
               .IsRequired();

        builder.HasMany(x => x.Amortizaciones)
               .WithOne()
               .HasForeignKey(x => new { x.IdEmpresa, x.TipoOperacion, x.NroOperacion })
               .IsRequired();
    }
}
