using Asp.Versioning;
using DataConsulting.PuntoVentaComercial.API.Utils;
using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Features.Auth.Commands.Login;
using DataConsulting.PuntoVentaComercial.Application.Features.Auth.Queries.GetSessionEnvironment;
using DataConsulting.PuntoVentaComercial.Application.Features.Auth.Queries.GetUserPolicies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DataConsulting.PuntoVentaComercial.API.Controllers.Auth
{
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}")]
    public class AuthController : ControllerBase
    {
        private readonly ICommandHandler<LoginCommand, LoginResponse> _loginHandler;
        private readonly IQueryHandler<GetUserPoliciesQuery, GetUserPoliciesResponse> _policiesHandler;
        private readonly IQueryHandler<GetSessionEnvironmentQuery, GetSessionEnvironmentResponse> _sessionHandler;

        public AuthController(
            ICommandHandler<LoginCommand, LoginResponse> loginHandler,
            IQueryHandler<GetUserPoliciesQuery, GetUserPoliciesResponse> policiesHandler,
            IQueryHandler<GetSessionEnvironmentQuery, GetSessionEnvironmentResponse> sessionHandler)
        {
            _loginHandler = loginHandler;
            _policiesHandler = policiesHandler;
            _sessionHandler = sessionHandler;
        }

        /// <summary>Autentica al usuario y retorna un JWT con claims de sesión.</summary>
        [HttpPost("auth/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new LoginCommand(
                request.Username,
                request.Password,
                request.IdEmpresa,
                request.CodigoEstacion);

            var result = await _loginHandler.Handle(command, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
        }

        /// <summary>Retorna las políticas activas del usuario autenticado.</summary>
        [HttpGet("session/policies")]
        [Authorize]
        public async Task<IActionResult> GetPolicies(CancellationToken cancellationToken = default)
        {
            int idUsuario = GetClaimInt("sub");

            var query = new GetUserPoliciesQuery(idUsuario);
            var result = await _policiesHandler.Handle(query, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        /// <summary>Retorna los datos completos del entorno de sesión (empresa, sucursal, estación, constantes).</summary>
        [HttpGet("session/environment")]
        [Authorize]
        public async Task<IActionResult> GetEnvironment(CancellationToken cancellationToken = default)
        {
            var query = new GetSessionEnvironmentQuery(
                IdEmpresa:    GetClaimInt("empresa_id"),
                IdSucursal:   GetClaimInt("sucursal_id"),
                IdEstacion:   GetClaimInt("estacion_id"),
                IdTrabajador: GetClaimInt("trabajador_id"));

            var result = await _sessionHandler.Handle(query, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        private int GetClaimInt(string type) =>
            int.Parse(User.FindFirst(type)?.Value ?? "0");
    }
}
