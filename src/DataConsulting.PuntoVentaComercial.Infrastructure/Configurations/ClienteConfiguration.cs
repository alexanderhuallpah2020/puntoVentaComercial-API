using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Configurations;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Cliente", "dbo", tb => tb.UseSqlOutputClause(false));
        builder.HasKey(x => x.Id);
        // IdCliente es IDENTITY(1,1) — EF asigna el Id tras el INSERT
        // UseSqlOutputClause(false) necesario porque la tabla tiene triggers
        builder.Property(x => x.Id).HasColumnName("IdCliente").ValueGeneratedOnAdd();

        builder.Property(x => x.IdTipoCliente).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.NombreComercial).HasMaxLength(100);
        builder.Property(x => x.IdDocumentoIdentidad).HasColumnType("smallint");
        builder.Property(x => x.NumDocumento).HasMaxLength(20);
        builder.Property(x => x.CodValidadorDoc).HasMaxLength(3);
        builder.Property(x => x.EstadoCliente).HasMaxLength(1).IsFixedLength().IsRequired();
        builder.Property(x => x.Observaciones).HasMaxLength(200);

        builder.Property(x => x.FlagTipoCliente).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.FlagSexo).HasColumnType("tinyint");
        builder.Property(x => x.FlagOperacion).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.FlagCertCalidad).HasColumnType("tinyint").IsRequired();
        builder.Property(x => x.FlagCredito).HasColumnType("smallint").IsRequired();
        builder.Property(x => x.FlagTipoCalificacion).HasColumnType("smallint").IsRequired();

        builder.Property(x => x.CreditoMaximo).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.IdTipoMoneda);
        builder.Property(x => x.IdFormaPago).HasColumnType("smallint");
        builder.Property(x => x.IdGiroNegocio);
        builder.Property(x => x.IdPais).HasColumnType("smallint").IsRequired();

        builder.Property(x => x.IdTrabajadorRef).HasColumnType("smallint");
        builder.Property(x => x.IdClienteRef);
        builder.Property(x => x.IdEntidadRef);

        builder.Property(x => x.NombreOwner).HasMaxLength(100);
        builder.Property(x => x.IdDocumentoIdentidadOwner).HasColumnType("smallint");
        builder.Property(x => x.NumDocumentoOwner).HasMaxLength(20);
        builder.Property(x => x.FechaNacimientoOwner);

        builder.Property(x => x.FechaAlta).HasColumnType("smalldatetime");
        builder.Property(x => x.FechaBaja).HasColumnType("smalldatetime");

        builder.Property(x => x.UsuarioCreador).HasMaxLength(20).IsRequired();
        // CORRECCIÓN CRÍTICA: columna se llama FechaCreador (no FechaCreacion)
        builder.Property(x => x.FechaCreacion).HasColumnName("FechaCreador").IsRequired();
        builder.Property(x => x.UsuarioModificador).HasMaxLength(20);
        builder.Property(x => x.FechaModificacion).HasColumnType("smalldatetime");

        // Shadow properties para columnas NOT NULL sin representación en la entidad
        builder.Property<string>("CondicionPago").HasMaxLength(100).IsRequired();
        builder.Property<byte>("FlagClientesVarios").HasColumnType("tinyint").IsRequired();

        builder.HasMany(x => x.ClienteLocales)
               .WithOne()
               .HasForeignKey(x => x.IdCliente);
    }
}
