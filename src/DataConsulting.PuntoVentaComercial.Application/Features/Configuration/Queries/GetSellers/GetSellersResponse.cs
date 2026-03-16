namespace DataConsulting.PuntoVentaComercial.Application.Features.Configuration.Queries.GetSellers
{
    public sealed record GetSellersResponse(IReadOnlyList<SellerDto> Sellers);

    public sealed record SellerDto(
        int Id,
        string Nombres,
        string Apellidos,
        string NombreCompleto,
        string? Codigo,
        decimal PorcentajeDescuentoMaximo
    );
}
