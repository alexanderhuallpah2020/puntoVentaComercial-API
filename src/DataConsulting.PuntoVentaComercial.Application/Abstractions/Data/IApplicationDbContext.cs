using DataConsulting.PuntoVentaComercial.Domain.ClasesSunat;
using DataConsulting.PuntoVentaComercial.Domain.FamiliasSunat;
using DataConsulting.PuntoVentaComercial.Domain.SegmentosSunat;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Application.Abstractions.Data
{
    public interface IApplicationDbContext
    {
        DbSet<SegmentoSunat> SegmentosSunat { get; }
        DbSet<FamiliaSunat> FamiliasSunat { get; }
        DbSet<ClaseSunat> ClasesSunat { get; }
    }
}