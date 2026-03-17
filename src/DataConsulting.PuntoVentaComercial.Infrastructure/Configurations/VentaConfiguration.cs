using DataConsulting.PuntoVentaComercial.Domain.Ventas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("Venta", "dbo", tb => tb.UseSqlOutputClause(false));
        builder.HasKey(x => x.Id);
        // IdVenta es IDENTITY(1,1) — UseSqlOutputClause(false) por trigger TR_Venta_Documento
        builder.Property(x => x.Id).HasColumnName("IdVenta").ValueGeneratedOnAdd();

        builder.Property(x => x.IdEmpresa).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdSucursal).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdEstacionTrabajo).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdSubSede).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdTipoDocumento).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.NumSerie).HasColumnType("smallint");
        builder.Property(x => x.NumeroDocumento).HasColumnType("int");
        builder.Property(x => x.NroCorrelativo).HasColumnType("int");

        builder.Property(x => x.IdCliente).IsRequired();
        builder.Property(x => x.IdTipoCliente).HasColumnType("smallint");
        builder.Property(x => x.Vendedor).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.Vendedor2).HasColumnType("smallint");

        builder.Property(x => x.FechaEmision).HasColumnType("smalldatetime").IsRequired();
        builder.Property(x => x.FechaProceso).HasColumnType("smalldatetime").IsRequired();
        builder.Property(x => x.Estado).HasMaxLength(1).IsFixedLength().IsRequired();

        builder.Property(x => x.IdTipoMoneda).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.TipoCambio).HasColumnType("smallmoney").IsRequired();

        builder.Property(x => x.ValorNeto).HasColumnType("money").IsRequired();
        builder.Property(x => x.ImporteDescuento).HasColumnType("money").IsRequired();
        builder.Property(x => x.ImporteDescuentoGlobal).HasColumnType("money").IsRequired();
        builder.Property(x => x.PorcentajeDescuentoGlobal).HasColumnType("decimal(10,4)");
        // CORRECCIÓN: columna se llama "Valorventa" (v minúscula)
        builder.Property(x => x.ValorVenta).HasColumnName("Valorventa").HasColumnType("money").IsRequired();
        builder.Property(x => x.Igv).HasColumnName("IGV").HasColumnType("money").IsRequired();
        builder.Property(x => x.ValorExonerado).HasColumnType("money").IsRequired();
        builder.Property(x => x.ImporteTotal).HasColumnType("money").IsRequired();
        builder.Property(x => x.ImportePagado).HasColumnType("money").IsRequired();
        builder.Property(x => x.ImporteVuelto).HasColumnType("money").IsRequired();
        builder.Property(x => x.Redondeo).HasColumnType("money").IsRequired();
        builder.Property(x => x.Isc).HasColumnName("ISC").HasColumnType("money").IsRequired();
        builder.Property(x => x.ValorICBPER).HasColumnType("smallmoney").IsRequired();

        builder.Property(x => x.IdTipoVenta).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdFormaPago).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.IdTurnoAsistencia).HasColumnType("smallint");
        builder.Property(x => x.IdSubdiario).HasColumnType("smallint");
        builder.Property(x => x.FlagDescPorcentaje).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.FlagPagoAdelantado).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.FlagDetraccion).HasColumnType("tinyint").IsRequired();

        builder.Property(x => x.UsuarioInsert).HasMaxLength(20).IsRequired();
        builder.Property(x => x.FechaInsert).HasColumnType("smalldatetime").IsRequired();
        builder.Property(x => x.UsuarioUpdate).HasMaxLength(20);
        builder.Property(x => x.FechaUpdate).HasColumnType("smalldatetime");
        builder.Property(x => x.UpdateToken).HasColumnType("tinyint").IsConcurrencyToken();

        // Shadow properties para columnas NOT NULL sin representación en la entidad
        builder.Property<decimal>("DescuentoDetalle").HasColumnType("money").HasDefaultValue(0m);
        builder.Property<decimal>("ValorRegalo").HasColumnType("money").HasDefaultValue(0m);
        builder.Property<byte>("FlagImpresion").HasColumnType("tinyint").HasDefaultValue((byte)0);
        builder.Property<byte>("FlagPrecio").HasColumnType("tinyint").HasDefaultValue((byte)0);
        builder.Property<byte>("FlagCobranzaDudosa").HasColumnType("tinyint").HasDefaultValue((byte)0);
        builder.Property<short>("IGVFactor").HasColumnType("smallint").HasDefaultValue((short)1800);
        builder.Property<byte>("FlagValorCambio").HasColumnType("tinyint").HasDefaultValue((byte)0);
        builder.Property<byte>("EstadoContable").HasColumnType("tinyint").HasDefaultValue((byte)0);
        builder.Property<bool>("FlagIGVAfecto").HasColumnType("bit").HasDefaultValue(true);
        builder.Property<byte>("ImpresionCuenta").HasColumnType("tinyint").HasDefaultValue((byte)0);
        builder.Property<bool>("FlagBoletaItinerante").HasColumnType("bit").HasDefaultValue(false);
        builder.Property<bool>("FlagComercioExterior").HasColumnType("bit").HasDefaultValue(false);

        builder.HasMany(x => x.Detalles)
               .WithOne()
               .HasForeignKey(x => x.IdVenta)
               .IsRequired();

        builder.HasMany(x => x.Pagos)
               .WithOne()
               .HasForeignKey(x => x.IdVenta)
               .IsRequired();
    }
}
