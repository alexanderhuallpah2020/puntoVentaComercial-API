using DataConsulting.PuntoVentaComercial.Application.Services.Auth;
using DataConsulting.PuntoVentaComercial.Domain.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataConsulting.PuntoVentaComercial.Infrastructure.Auth
{
    internal sealed class JwtTokenService : IJwtTokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationHours;

        public JwtTokenService(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey no configurada.");
            _issuer = configuration["Jwt:Issuer"] ?? "PuntoVentaComercial";
            _audience = configuration["Jwt:Audience"] ?? "PuntoVentaComercialClient";
            _expirationHours = int.TryParse(configuration["Jwt:ExpirationHours"], out var h) ? h : 8;
        }

        public (string Token, DateTimeOffset ExpiresAt) GenerateToken(
            User user,
            Workstation workstation,
            List<string> policies)
        {
            var expiresAt = DateTimeOffset.UtcNow.AddHours(_expirationHours);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,        user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Username),
                new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
                new("empresa_id",        user.IdEmpresa.ToString()),
                new("sucursal_id",       workstation.IdSucursal.ToString()),
                new("sucursal_nombre",   workstation.NombreSucursal),
                new("estacion_id",       workstation.Id.ToString()),
                new("estacion_codigo",   workstation.Codigo),
                new("trabajador_id",     user.IdTrabajador.ToString()),
                new("trabajador_nombre", user.NombreTrabajador)
            };

            foreach (var policy in policies)
                claims.Add(new Claim("policy", policy));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expiresAt.UtcDateTime,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}
