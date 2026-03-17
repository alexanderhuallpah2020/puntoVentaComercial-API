using DataConsulting.PuntoVentaComercial.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class PaymentDetailConfiguration : IEntityTypeConfiguration<PaymentDetail>
    {
        public void Configure(EntityTypeBuilder<PaymentDetail> builder)
        {
            builder.ToTable("OperacionPagoDetalle", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IdDetalle")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.IdOperacion).HasColumnName("IdOperacion").IsRequired();

            builder.Property(x => x.IdFormaPago).HasColumnName("IdFormaPago").IsRequired();

            builder.Property(x => x.Descripcion)
                .HasColumnName("Descripcion")
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Property(x => x.TipoMoneda)
                .HasColumnName("IdTipoMoneda")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.TipoCambio)
                .HasColumnName("TipoCambio")
                .HasColumnType("decimal(10,4)")
                .IsRequired();

            builder.Property(x => x.Importe)
                .HasColumnName("Importe")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.ImporteEnSoles)
                .HasColumnName("ImporteEnSoles")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }
}
