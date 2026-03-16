using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataConsulting.PuntoVentaComercial.Application.Services.ClasesSunat
{
    public interface IClaseSunatService
    {
        Task<Result<int>> RegistrarClaseAsync(
            int idFamiliaSunat,
            string codigo,
            string descripcion,
            short idUsuario,
            CancellationToken ct);
    }
}
