using DataConsulting.PuntoVentaComercial.Domain.ClasesSunat;
using DataConsulting.PuntoVentaComercial.Domain.Clients;
using DataConsulting.PuntoVentaComercial.Domain.FamiliasSunat;
using DataConsulting.PuntoVentaComercial.Domain.Payments;
using DataConsulting.PuntoVentaComercial.Domain.Sales;
using DataConsulting.PuntoVentaComercial.Domain.SegmentosSunat;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Data
{
    public interface IApplicationDbContext
    {
        DbSet<SegmentoSunat> SegmentosSunat { get; }
        DbSet<FamiliaSunat> FamiliasSunat { get; }
        DbSet<ClaseSunat> ClasesSunat { get; }
        DbSet<Client> Clients { get; }
        DbSet<ClientLocal> ClientLocals { get; }
        DbSet<Sale> Sales { get; }
        DbSet<SaleItem> SaleItems { get; }
        DbSet<Payment> Payments { get; }
        DbSet<PaymentDetail> PaymentDetails { get; }
    }
}