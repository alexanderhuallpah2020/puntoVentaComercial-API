using DataConsulting.PuntoVentaComercial.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations
{
    internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.ToTable("Cliente", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("IdCliente")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Nombre)
                .HasColumnName("Nombre")
                .HasMaxLength(150)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.NombreComercial)
                .HasColumnName("NombreComercial")
                .HasMaxLength(150)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.IdDocumentoIdentidad)
                .HasColumnName("IdDocumentoIdentidad")
                .HasConversion<byte>()
                .IsRequired();

            builder.Property(x => x.NumDocumento)
                .HasColumnName("NumDocumento")
                .HasMaxLength(20)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.CodValidadorDoc)
                .HasColumnName("CodValidadorDoc")
                .HasMaxLength(1)
                .IsUnicode(false);

            builder.Property(x => x.IdPais)
                .HasColumnName("IdPais")
                .IsRequired();

            builder.Property(x => x.IdTipoCliente)
                .HasColumnName("IdTipoCliente")
                .IsRequired();

            builder.Property(x => x.FlagIGV)
                .HasColumnName("FlagIGV")
                .IsRequired();

            builder.Property(x => x.CreditoMaximo)
                .HasColumnName("CreditoMaximo")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.EstadoCliente)
                .HasColumnName("EstadoCliente")
                .HasMaxLength(1)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(x => x.FechaAlta)
                .HasColumnName("FechaAlta")
                .HasColumnType("smalldatetime")
                .IsRequired();

            builder.Property(x => x.FechaBaja)
                .HasColumnName("FechaBaja")
                .HasColumnType("smalldatetime");

            builder.Property(x => x.UpdateToken)
                .HasColumnName("UpdateToken")
                .HasColumnType("smallint")
                .IsRequired()
                .IsConcurrencyToken();

            builder.Property(x => x.IdUsuarioCreador)
                .HasColumnName("IdUsuarioCreador")
                .IsRequired();

            builder.Property(x => x.FechaCreacion)
                .HasColumnName("FechaCreacion")
                .HasColumnType("smalldatetime")
                .IsRequired();

            builder.Property(x => x.IdUsuarioModificador)
                .HasColumnName("IdUsuarioModificador");

            builder.Property(x => x.FechaModificacion)
                .HasColumnName("FechaModificacion")
                .HasColumnType("smalldatetime");

            builder.HasMany<ClientLocal>("_locals")
                .WithOne()
                .HasForeignKey(l => l.IdCliente);

            builder.Navigation("_locals")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .AutoInclude(false);
        }
    }
}
