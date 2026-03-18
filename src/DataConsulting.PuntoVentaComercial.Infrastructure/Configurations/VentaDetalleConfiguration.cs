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
        builder.Property(x => x.IdEmpresa).HasColumnType("smallint").IsRequired();

        builder.Property(x => x.IdArticulo);                                              // INT NULL
        builder.Property(x => x.IdUnidad).HasColumnType("smallint");                      // SMALLINT NULL
        builder.Property(x => x.DescripcionArticulo).HasMaxLength(500);

        builder.Property(x => x.Cantidad).HasColumnType("decimal(10,4)");                 // DECIMAL NULL
        builder.Property(x => x.PrecioUnitario).HasColumnType("decimal(18,9)").IsRequired();
        builder.Property(x => x.ValorUnitario).HasColumnType("decimal(12,4)");            // DECIMAL NULL
        builder.Property(x => x.ValorVenta).HasColumnType("money").IsRequired();
        builder.Property(x => x.ValorFacial).HasColumnType("money").IsRequired();
        builder.Property(x => x.ImporteDescuento).HasColumnType("money").IsRequired();
        builder.Property(x => x.TipoDescuento).HasColumnType("tinyint").IsRequired();

        // IGV en DetalleVenta es BIT (flag), distinto del IGV MONEY en Venta
        builder.Property(x => x.Igv).HasColumnName("IGV").HasColumnType("bit").IsRequired();
        builder.Property(x => x.FlagExonerado).HasColumnType("bit").IsRequired();
        builder.Property(x => x.Isc).HasColumnName("ISC").HasColumnType("money");          // MONEY NULL
        builder.Property(x => x.IdTipoAfectoIGV).HasColumnType("int");
        builder.Property(x => x.ValorICBPER).HasColumnType("smallmoney").IsRequired();
        builder.Property(x => x.FlagRegalo).HasColumnType("tinyint");                      // TINYINT NULL

        builder.Property(x => x.IdUsuarioCreador).HasColumnType("smallint");              // SMALLINT NULL
        builder.Property(x => x.FechaCreacion).HasColumnType("smalldatetime");            // SMALLDATETIME NULL

        // Shadow properties — sin DEFAULT en BD: ValueGeneratedNever() fuerza EF a incluir el valor en INSERT
        builder.Property<decimal>("ImporteEventa").HasColumnType("money").HasDefaultValue(0m).ValueGeneratedNever();
        builder.Property<decimal?>("TasaISC").HasColumnType("money");                           // MONEY NULL
        builder.Property<short?>("TipoISC").HasColumnType("smallint");                          // SMALLINT NULL
        builder.Property<decimal?>("PorPercepcion").HasColumnType("money");                     // MONEY NULL
        builder.Property<decimal?>("MontoPercepcion").HasColumnType("money");                   // MONEY NULL
        builder.Property<byte>("FlagPagoAdelantadoDet").HasColumnType("tinyint").HasDefaultValue((byte)0).ValueGeneratedNever();
        builder.Property<byte>("FlagDetalle").HasColumnType("tinyint").HasDefaultValue((byte)0).ValueGeneratedNever();
        builder.Property<byte>("TipoRegalo").HasColumnType("tinyint").HasDefaultValue((byte)0).ValueGeneratedNever();
        builder.Property<bool>("FlagICBPER").HasColumnType("bit").HasDefaultValue(false);       // BD tiene DEFAULT((0)): EF puede omitirlo
    }
}
