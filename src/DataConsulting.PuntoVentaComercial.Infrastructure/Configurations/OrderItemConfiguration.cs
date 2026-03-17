using DataConsulting.PuntoVentaComercial.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("PedidoDetalle", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IdDetalle")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.IdPedido).HasColumnName("IdPedido").IsRequired();

            builder.Property(x => x.IdArticulo).HasColumnName("IdArticulo").IsRequired();

            builder.Property(x => x.Codigo)
                .HasColumnName("Codigo")
                .HasMaxLength(30)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Descripcion)
                .HasColumnName("Descripcion")
                .HasMaxLength(200)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.SiglaUnidad)
                .HasColumnName("SiglaUnidad")
                .HasMaxLength(10)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.IdUnidad).HasColumnName("IdUnidad").IsRequired();

            builder.Property(x => x.Cantidad)
                .HasColumnName("Cantidad")
                .HasColumnType("decimal(18,4)")
                .IsRequired();

            builder.Property(x => x.PrecioUnitario)
                .HasColumnName("PrecioUnitario")
                .HasColumnType("decimal(18,4)")
                .IsRequired();

            builder.Property(x => x.TipoAfectacionIgv)
                .HasColumnName("IdAfectacionIgv")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ValorVenta)
                .HasColumnName("ValorVenta")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Descuento)
                .HasColumnName("Descuento")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.ValorVentaNeto)
                .HasColumnName("ValorVentaNeto")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Isc)
                .HasColumnName("ISC")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Igv)
                .HasColumnName("IGV")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Icbper)
                .HasColumnName("ValorICBPER")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Subtotal)
                .HasColumnName("SubTotal")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.IdClaseProducto).HasColumnName("IdClaseProducto").IsRequired();

            builder.Property(x => x.IdTipoCliente).HasColumnName("IdTipoCliente").IsRequired();

            builder.Property(x => x.Estado).HasColumnName("Estado").IsRequired();
        }
    }
}
