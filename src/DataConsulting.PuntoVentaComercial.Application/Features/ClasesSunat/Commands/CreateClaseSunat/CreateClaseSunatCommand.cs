using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataConsulting.PuntoVentaComercial.Application.Features.ClasesSunat.Commands.CreateClaseSunat
{
    public sealed record CreateClaseSunatCommand(
        int IdFamiliaSunat,
        string Codigo,
        string Descripcion,
        short IdUsuarioCreador
    ) : ICommand<int>;
}
