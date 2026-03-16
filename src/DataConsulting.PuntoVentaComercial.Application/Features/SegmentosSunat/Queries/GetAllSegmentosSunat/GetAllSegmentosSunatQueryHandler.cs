using DataConsulting.PuntoVentaComercial.Application.Abstractions.Data;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetAllSegmentosSunat
{
    internal sealed class GetAllSegmentosSunatQueryHandler(IApplicationDbContext context)
        : IQueryHandler<GetAllSegmentosSunatQuery, List<GetAllSegmentosSunatResponse>>
    {

        public async Task<Result<List<GetAllSegmentosSunatResponse>>> Handle(GetAllSegmentosSunatQuery query, CancellationToken cancellationToken)
        {

            List<GetAllSegmentosSunatResponse> todos = await context.SegmentosSunat
                .Select(x => new GetAllSegmentosSunatResponse
                {
                    IdSegmentoSunat = x.IdSegmentoSunat,
                    Codigo = x.Codigo,
                    Descripcion = x.Descripcion,
                    Estado = (short)x.Estado,      // EEstado -> short (porque BD smallint)
                    UpdateToken = x.UpdateToken,
                    IdUsuarioCreador = x.IdUsuarioCreador,
                    FechaCreacion = x.FechaCreacion,
                    IdUsuarioModificador = x.IdUsuarioModificador,
                    FechaModificacion = x.FechaModificacion
                })
                .ToListAsync(cancellationToken);

            return todos;
        }
    }
}
