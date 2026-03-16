using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.ClasesSunat;
using DataConsulting.PuntoVentaComercial.Domain.FamiliasSunat;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataConsulting.PuntoVentaComercial.Application.Services.ClasesSunat
{
    internal sealed class ClaseSunatService(
       IClaseSunatRepository claseRepository,
       IFamiliaSunatRepository familiaRepository) : IClaseSunatService
    {
        public async Task<Result<int>> RegistrarClaseAsync(
            int idFamiliaSunat,
            string codigo,
            string descripcion,
            short idUsuario,
            CancellationToken ct)
        {

            var familia = await familiaRepository.GetByIdAsync(idFamiliaSunat, ct);
            if (familia is null)
            {
                return Result.Failure<int>(ClaseSunatErrors.FamiliaNotFound(idFamiliaSunat));
            }


            if (await claseRepository.ExistsByCodigoAsync(idFamiliaSunat, codigo, ct))
            {
                return Result.Failure<int>(ClaseSunatErrors.CodigoDuplicado(codigo));
            }
               
            int nuevoId = await claseRepository.GetNextIdAsync(ct);

            var result = ClaseSunat.Create(
                nuevoId, 
                idFamiliaSunat, 
                codigo, 
                descripcion, 
                1, 
                idUsuario, 
                DateTime.UtcNow);

            if (result.IsFailure)
            {
                return Result.Failure<int>(result.Error);
            }

            claseRepository.Add(result.Value);

            return Result.Success(result.Value.Id);
        }
    }
}
