using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetPriceList;
using DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetProductDetail;
using DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetProductStock;
using DataConsulting.PuntoVentaComercial.Application.Features.Products.Queries.GetTopSellers;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Products
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/products")]
    //[Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IQueryHandler<GetPriceListQuery, GetPriceListResponse> _priceListHandler;
        private readonly IQueryHandler<GetTopSellersQuery, GetTopSellersResponse> _topSellersHandler;
        private readonly IQueryHandler<GetProductDetailQuery, GetProductDetailResponse> _detailHandler;
        private readonly IQueryHandler<GetProductStockQuery, GetProductStockResponse> _stockHandler;

        public ProductsController(
            IQueryHandler<GetPriceListQuery, GetPriceListResponse> priceListHandler,
            IQueryHandler<GetTopSellersQuery, GetTopSellersResponse> topSellersHandler,
            IQueryHandler<GetProductDetailQuery, GetProductDetailResponse> detailHandler,
            IQueryHandler<GetProductStockQuery, GetProductStockResponse> stockHandler)
        {
            _priceListHandler = priceListHandler;
            _topSellersHandler = topSellersHandler;
            _detailHandler = detailHandler;
            _stockHandler = stockHandler;
        }

        /// <summary>
        /// Lista de precios para el POS. Filtros: codigo, descripcion, sucursal, tipoCliente, soloConStock.
        /// </summary>
        [HttpGet("price-list")]
        public async Task<IActionResult> GetPriceList(
            [FromQuery] int idSucursal,
            [FromQuery] int idTipoCliente = 1,
            [FromQuery] string? codigo = null,
            [FromQuery] string? descripcion = null,
            [FromQuery] bool soloConStock = false,
            CancellationToken cancellationToken = default)
        {
            var query = new GetPriceListQuery(idSucursal, idTipoCliente, codigo, descripcion, soloConStock);
            var result = await _priceListHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Top artículos más vendidos en la sucursal (últimos 90 días). Para el panel rápido del POS.
        /// </summary>
        [HttpGet("top-sellers")]
        public async Task<IActionResult> GetTopSellers(
            [FromQuery] int idSucursal,
            [FromQuery] int top = 20,
            CancellationToken cancellationToken = default)
        {
            var query = new GetTopSellersQuery(idSucursal, top);
            var result = await _topSellersHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Detalle completo de un artículo incluyendo composición (si es compuesto).
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var query = new GetProductDetailQuery(id);
            var result = await _detailHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Stock disponible de un artículo en una sucursal.
        /// </summary>
        [HttpGet("{id:int}/stock")]
        public async Task<IActionResult> GetStock(
            int id,
            [FromQuery] int idSucursal,
            CancellationToken cancellationToken = default)
        {
            var query = new GetProductStockQuery(id, idSucursal);
            var result = await _stockHandler.Handle(query, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }
    }
}
