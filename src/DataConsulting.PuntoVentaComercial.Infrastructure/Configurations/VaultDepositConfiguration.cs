using DataConsulting.PuntoVentaComercial.Domain.Cash;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class VaultDepositConfiguration : IEntityTypeConfiguration<VaultDeposit>
    {
        public void Configure(EntityTypeBuilder<VaultDeposit> builder)
        {
            builder.ToTable("DepositoBoveda", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IdDepositoBoveda")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.IdEmpresa).HasColumnName("IdEmpresa").IsRequired();
            builder.Property(x => x.IdSucursal).HasColumnName("IdSucursal").IsRequired();
            builder.Property(x => x.IdTrabajador).HasColumnName("IdTrabajador").IsRequired();
            builder.Property(x => x.IdIsla).HasColumnName("IdIsla").IsRequired();
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

            builder.Property(x => x.NumDocumento)
                .HasColumnName("NumDocumento")
                .HasMaxLength(20)
                .IsRequired()
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

            builder.Property(x => x.Glosa)
                .HasColumnName("Glosa")
                .HasMaxLength(200)
                .IsUnicode(false);

            builder.Property(x => x.IdFormaPago).HasColumnName("IdFormaPago").IsRequired();

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
        }
    }
}
