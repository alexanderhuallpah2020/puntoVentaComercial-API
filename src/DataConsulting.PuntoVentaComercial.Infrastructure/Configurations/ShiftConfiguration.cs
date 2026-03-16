using DataConsulting.PuntoVentaComercial.Domain.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class ShiftConfiguration : IEntityTypeConfiguration<Shift>
    {
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            builder.ToTable("TurnoAsistencia");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("IdTurnoAsistencia");
            builder.Property(x => x.IdEmpresa).IsRequired();
            builder.Property(x => x.Descripcion).HasMaxLength(100).IsRequired();
            builder.Property(x => x.HoraInicio).IsRequired();
            builder.Property(x => x.HoraFin).IsRequired();
            builder.Property(x => x.Activo).HasColumnName("Estado").IsRequired();
        }
    }
}
