using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Sales.Commands.CalculateSale;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Sales
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/sales")]
    public class SalesController : ControllerBase
    {
        private readonly ICommandHandler<CalculateSaleCommand, CalculateSaleResponse> _calculateHandler;

        public SalesController(ICommandHandler<CalculateSaleCommand, CalculateSaleResponse> calculateHandler)
        {
            _calculateHandler = calculateHandler;
        }

        /// <summary>
        /// Calcula totales de una venta (IGV, ISC, ICBPER, descuentos) sin persistir datos.
        /// Determina automáticamente el tipo de documento (Factura/Boleta) según el documento del cliente.
        /// </summary>
        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate(
            [FromBody] CalculateSaleRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new CalculateSaleCommand(
                TipoDocumentoCliente: request.TipoDocumentoCliente,
                Items: request.Items.Select(i => new CalculateSaleItemCommand(
                    ProductoId: i.ProductoId,
                    Cantidad: i.Cantidad,
                    PrecioUnitario: i.PrecioUnitario,
                    TipoAfectacionIgv: i.TipoAfectacionIgv,
                    TipoIsc: i.TipoIsc,
                    TasaIsc: i.TasaIsc,
                    MontoFijoIsc: i.MontoFijoIsc,
                    EsBolsaPlastica: i.EsBolsaPlastica)).ToList(),
                DescuentoGlobal: request.DescuentoGlobal,
                FormaPago: request.FormaPago);

            var result = await _calculateHandler.Handle(command, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
    }
}
