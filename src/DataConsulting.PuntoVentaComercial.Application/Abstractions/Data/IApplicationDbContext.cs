using DataConsulting.PuntoVentaComercial.Domain.Cash;
using DataConsulting.PuntoVentaComercial.Domain.Clients;
using DataConsulting.PuntoVentaComercial.Domain.Orders;
using DataConsulting.PuntoVentaComercial.Domain.Payments;
using DataConsulting.PuntoVentaComercial.Domain.Sales;
using DataConsulting.PuntoVentaComercial.Domain.Sunat;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Client> Clients { get; }
        DbSet<ClientLocal> ClientLocals { get; }
        DbSet<Sale> Sales { get; }
        DbSet<SaleItem> SaleItems { get; }
        DbSet<Payment> Payments { get; }
        DbSet<PaymentDetail> PaymentDetails { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<VaultDeposit> VaultDeposits { get; }
        DbSet<SunatSubmission> SunatSubmissions { get; }
    }
}