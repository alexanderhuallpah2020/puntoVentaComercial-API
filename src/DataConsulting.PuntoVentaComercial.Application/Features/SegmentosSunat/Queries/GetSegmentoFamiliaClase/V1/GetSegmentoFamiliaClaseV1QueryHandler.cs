using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase.V1
{
    internal sealed class GetSegmentoFamiliaClaseV1QueryHandler(IApplicationDbContext context)
            : IQueryHandler<GetSegmentoFamiliaClaseV1Query, List<GetSegmentoFamiliaClaseResponse>>
    {
        public async Task<Result<List<GetSegmentoFamiliaClaseResponse>>> Handle(
            GetSegmentoFamiliaClaseV1Query query,
            CancellationToken cancellationToken)
        {
            var data =
                await (from s in context.SegmentosSunat.AsNoTracking()
                       join f in context.FamiliasSunat.AsNoTracking()
                           on s.Id equals f.IdSegmentoSunat
                       join c in context.ClasesSunat.AsNoTracking()
                           on f.Id equals c.IdFamiliaSunat
                       orderby s.Codigo, f.Codigo, c.Codigo
                       select new GetSegmentoFamiliaClaseResponse
                       {
                           Segmento = s.Codigo,
                           SegmentoDescripcion = s.Descripcion,
                           Familia = f.Codigo,
                           FamiliaDescripcion = f.Descripcion,
                           Clase = c.Codigo,
                           ClaseDescripcion = c.Descripcion
                       })
                      .ToListAsync(cancellationToken);

            return data;
        }
    }
}
