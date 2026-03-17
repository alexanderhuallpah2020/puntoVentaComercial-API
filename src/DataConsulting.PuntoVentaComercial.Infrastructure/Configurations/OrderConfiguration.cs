using DataConsulting.PuntoVentaComercial.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Pedido", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IdPedido")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.IdEmpresa).HasColumnName("IdEmpresa").IsRequired();
            builder.Property(x => x.IdSucursal).HasColumnName("IdSucursal").IsRequired();
            builder.Property(x => x.IdEstacion).HasColumnName("IdEstacion").IsRequired();
            builder.Property(x => x.IdTurnoAsistencia).HasColumnName("IdTurnoAsistencia").IsRequired();

            builder.Property(x => x.FechaEmision)
                .HasColumnName("FechaEmision")
                .HasColumnType("smalldatetime")
                .IsRequired();

            builder.Property(x => x.TipoDocumento)
                .HasColumnName("IdTipoDocumento")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.NumSerie)
                .HasColumnName("NumSerie")
                .HasMaxLength(10)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.Correlativo).HasColumnName("Correlativo").IsRequired();

            builder.Property(x => x.IdCliente).HasColumnName("IdCliente").IsRequired();

            builder.Property(x => x.NombreCliente)
                .HasColumnName("NombreCliente")
                .HasMaxLength(150)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.IdDocumentoIdentidad)
                .HasColumnName("IdDocumentoIdentidad")
                .IsRequired();

            builder.Property(x => x.NumDocumentoCliente)
                .HasColumnName("NumDocumentoCliente")
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(x => x.DireccionCliente)
                .HasColumnName("DireccionCliente")
                .HasMaxLength(300)
                .IsUnicode(false);

            builder.Property(x => x.FlagIGV).HasColumnName("FlagIGV").IsRequired();

            builder.Property(x => x.IdTrabajador).HasColumnName("IdTrabajador").IsRequired();
            builder.Property(x => x.IdTrabajador2).HasColumnName("IdTrabajador2");

            builder.Property(x => x.TipoMoneda)
                .HasColumnName("IdTipoMoneda")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.TipoCambio)
                .HasColumnName("TipoCambio")
                .HasColumnType("decimal(10,4)")
                .IsRequired();

            builder.Property(x => x.DescuentoGlobal)
                .HasColumnName("DescuentoGlobal")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.ValorAfecto)
                .HasColumnName("ValorAfecto")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.ValorInafecto)
                .HasColumnName("ValorInafecto")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.ValorExonerado)
                .HasColumnName("ValorExonerado")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.ValorGratuito)
                .HasColumnName("ValorGratuito")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.TotalIsc)
                .HasColumnName("ISC")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Igv)
                .HasColumnName("IGV")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.TotalIcbper)
                .HasColumnName("ValorICBPER")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.ImporteTotal)
                .HasColumnName("ImporteTotal")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Observaciones)
                .HasColumnName("Observaciones")
                .HasMaxLength(500)
                .IsUnicode(false);

            builder.Property(x => x.Estado).HasColumnName("Estado").IsRequired();

            builder.Property(x => x.UpdateToken)
                .HasColumnName("UpdateToken")
                .HasColumnType("smallint")
                .IsRequired()
                .IsConcurrencyToken();

            builder.Property(x => x.IdUsuarioCreador).HasColumnName("IdUsuarioCreador").IsRequired();

            builder.Property(x => x.FechaCreacion)
                .HasColumnName("FechaCreacion")
                .HasColumnType("smalldatetime")
                .IsRequired();

            builder.Property(x => x.IdUsuarioModificador).HasColumnName("IdUsuarioModificador");

            builder.Property(x => x.FechaModificacion)
                .HasColumnName("FechaModificacion")
                .HasColumnType("smalldatetime");

            builder.HasMany<OrderItem>("_items")
                .WithOne()
                .HasForeignKey(i => i.IdPedido);

            builder.Navigation("_items")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .AutoInclude(false);
        }
    }
}
