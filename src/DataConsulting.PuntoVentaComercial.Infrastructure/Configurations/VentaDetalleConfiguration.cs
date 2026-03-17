using DataConsulting.PuntoVentaComercial.Domain.Ventas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class VentaDetalleConfiguration : IEntityTypeConfiguration<VentaDetalle>
{
    public void Configure(EntityTypeBuilder<VentaDetalle> builder)
    {
        builder.ToTable("DetalleVenta", "dbo");
        // PK compuesta: (IdVenta DESC, Correlativo ASC) — EF no necesita el orden del índice
        builder.HasKey(x => new { x.IdVenta, x.Correlativo });

        builder.Property(x => x.IdVenta).IsRequired();
        builder.Property(x => x.Correlativo).HasColumnType("smallint").IsRequired();

        builder.Property(x => x.IdArticulo).IsRequired();
        builder.Property(x => x.IdUnidad).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.DescripcionArticulo).HasMaxLength(500);

        builder.Property(x => x.Cantidad).HasColumnType("decimal(10,4)").IsRequired();
        builder.Property(x => x.PrecioUnitario).HasColumnType("decimal(18,9)").IsRequired();
        builder.Property(x => x.ValorUnitario).HasColumnType("decimal(12,4)").IsRequired();
        builder.Property(x => x.ValorVenta).HasColumnType("money").IsRequired();
        builder.Property(x => x.ValorFacial).HasColumnType("money").IsRequired();
        builder.Property(x => x.ImporteDescuento).HasColumnType("money").IsRequired();
        builder.Property(x => x.TipoDescuento).HasColumnType("tinyint").IsRequired();

        // IGV en DetalleVenta es BIT (flag), distinto del IGV MONEY en Venta
        builder.Property(x => x.Igv).HasColumnName("IGV").HasColumnType("bit").IsRequired();
        builder.Property(x => x.FlagExonerado).HasColumnType("bit").IsRequired();
        builder.Property(x => x.Isc).HasColumnName("ISC").HasColumnType("money").IsRequired();
        builder.Property(x => x.IdTipoAfectoIGV).HasColumnType("int");
        builder.Property(x => x.ValorICBPER).HasColumnType("smallmoney").IsRequired();
        builder.Property(x => x.FlagRegalo).HasColumnType("tinyint").IsRequired();

        builder.Property(x => x.IdUsuarioCreador).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.FechaCreacion).HasColumnType("smalldatetime").IsRequired();

        // Shadow properties para columnas NOT NULL sin representación en la entidad
        builder.Property<decimal>("ImporteEventa").HasColumnType("money").HasDefaultValue(0m);
        builder.Property<decimal>("TasaISC").HasColumnType("money").HasDefaultValue(0m);
        builder.Property<short>("TipoISC").HasColumnType("smallint").HasDefaultValue((short)0);
        builder.Property<decimal>("PorPercepcion").HasColumnType("money").HasDefaultValue(0m);
        builder.Property<decimal>("MontoPercepcion").HasColumnType("money").HasDefaultValue(0m);
        builder.Property<byte>("FlagPagoAdelantadoDet").HasColumnType("tinyint").HasDefaultValue((byte)0);
        builder.Property<byte>("FlagDetalle").HasColumnType("tinyint").HasDefaultValue((byte)0);
        builder.Property<bool>("FlagICBPER").HasColumnType("bit").HasDefaultValue(false);
    }
}
