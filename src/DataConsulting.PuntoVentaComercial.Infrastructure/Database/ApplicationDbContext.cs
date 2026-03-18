using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Domain.ClasesSunat;
using DataConsulting.PuntoVentaComercial.Domain.Clientes;
using DataConsulting.PuntoVentaComercial.Domain.DocumentosIdentidad;
using DataConsulting.PuntoVentaComercial.Domain.FamiliasSunat;
using DataConsulting.PuntoVentaComercial.Domain.Paises;
using DataConsulting.PuntoVentaComercial.Domain.SegmentosSunat;
using DataConsulting.PuntoVentaComercial.Domain.Ventas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Database;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

    public DbSet<SegmentoSunat> SegmentosSunat { get; set; }
    public DbSet<FamiliaSunat> FamiliasSunat { get; set; }
    public DbSet<ClaseSunat> ClasesSunat { get; set; }
    public DbSet<Cliente>              Clientes             { get; set; }
    public DbSet<ClienteLocal>         ClienteLocales       { get; set; }
    public DbSet<DocumentoIdentidad>   DocumentosIdentidad  { get; set; }
    public DbSet<Pais>                 Paises               { get; set; }
    public DbSet<Venta>                Ventas               { get; set; }
    public DbSet<VentaDetalle>         VentaDetalles        { get; set; }
    public DbSet<VentaPago>            VentaPagos           { get; set; }
    public DbSet<VentaCuota>           VentaCuotas          { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyShadowPropertyDefaults();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyShadowPropertyDefaults()
    {
        foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
        {
            foreach (var property in entry.Properties)
            {
                if (property.CurrentValue is null && property.Metadata.GetDefaultValue() is { } defaultValue)
                    property.CurrentValue = defaultValue;
            }
        }
    }

    public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (Database.CurrentTransaction is not null)
        {
            await Database.CurrentTransaction.DisposeAsync();
        }

        return (await Database.BeginTransactionAsync(cancellationToken)).GetDbTransaction();
    }
}