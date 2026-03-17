using DataConsulting.PuntoVentaComercial.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("OperacionPago", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IdOperacion")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.IdVenta).HasColumnName("IdVenta").IsRequired();
            builder.Property(x => x.IdEmpresa).HasColumnName("IdEmpresa").IsRequired();
            builder.Property(x => x.IdSucursal).HasColumnName("IdSucursal").IsRequired();
            builder.Property(x => x.IdCliente).HasColumnName("IdCliente").IsRequired();

            builder.Property(x => x.FechaRegistro)
                .HasColumnName("FechaRegistro")
                .HasColumnType("smalldatetime")
                .IsRequired();

            builder.Property(x => x.ImporteTotal)
                .HasColumnName("ImporteTotal")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.ImportePagado)
                .HasColumnName("ImportePagado")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Vuelto)
                .HasColumnName("Vuelto")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Credito)
                .HasColumnName("Credito")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Estado).HasColumnName("Estado").IsRequired();

            builder.Property(x => x.IdUsuarioCreador).HasColumnName("IdUsuarioCreador").IsRequired();

            builder.Property(x => x.FechaCreacion)
                .HasColumnName("FechaCreacion")
                .HasColumnType("smalldatetime")
                .IsRequired();

            builder.HasMany<PaymentDetail>("_detalles")
                .WithOne()
                .HasForeignKey(d => d.IdOperacion);

            builder.Navigation("_detalles")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .AutoInclude(false);
        }
    }
}
