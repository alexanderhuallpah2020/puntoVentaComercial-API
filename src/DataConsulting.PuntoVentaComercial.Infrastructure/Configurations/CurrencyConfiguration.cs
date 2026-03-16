using DataConsulting.PuntoVentaComercial.Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("TipoMoneda");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("IdTipoMoneda");
            builder.Property(x => x.TipoMoneda).HasColumnName("IdTipoMonedaEnum").IsRequired();
            builder.Property(x => x.Codigo).HasMaxLength(5).IsRequired();
            builder.Property(x => x.Simbolo).HasMaxLength(5).IsRequired();
            builder.Property(x => x.Descripcion).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Activo).HasColumnName("Estado").IsRequired();
        }
    }
}
