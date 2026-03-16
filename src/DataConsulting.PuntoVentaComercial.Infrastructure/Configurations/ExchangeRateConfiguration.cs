using DataConsulting.PuntoVentaComercial.Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.ToTable("TipoCambio");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("IdTipoCambio");
            builder.Property(x => x.IdEmpresa).IsRequired();
            builder.Property(x => x.Fecha).IsRequired();
            builder.Property(x => x.TipoMoneda).HasColumnName("IdTipoMoneda").IsRequired();
            builder.Property(x => x.TipoCambioCompra).HasColumnType("decimal(10,4)").IsRequired();
            builder.Property(x => x.TipoCambioVenta).HasColumnType("decimal(10,4)").IsRequired();
        }
    }
}
