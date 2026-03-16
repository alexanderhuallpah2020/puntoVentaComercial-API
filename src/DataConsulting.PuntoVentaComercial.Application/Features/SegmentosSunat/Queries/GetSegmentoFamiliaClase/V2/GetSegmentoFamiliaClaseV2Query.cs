using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataConsulting.PuntoVentaComercial.Application.Features.SegmentosSunat.Queries.GetSegmentoFamiliaClase.V2
{
    public sealed record GetSegmentoFamiliaClaseV2Query()
           : IQuery<List<GetSegmentoFamiliaClaseResponse>>;
}
