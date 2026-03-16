using DataConsulting.PuntoVentaComercial.Application.Abstractions.Messaging;
using DataConsulting.PuntoVentaComercial.Application.Services.Auth;
using DataConsulting.PuntoVentaComercial.Domain.Abstractions;
using DataConsulting.PuntoVentaComercial.Domain.Identity;
using System.Security.Cryptography;
using System.Text;

namespace DataConsulting.PuntoVentaComercial.Application.Features.Auth.Commands.Login
{
    internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IWorkstationRepository _workstationRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IWorkstationRepository workstationRepository,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _workstationRepository = workstationRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result<LoginResponse>> Handle(
            LoginCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAndEmpresaAsync(
                command.Username, command.IdEmpresa, cancellationToken);

            if (user is null || !VerifyPassword(command.Password, user.PasswordHash))
                return Result.Failure<LoginResponse>(IdentityErrors.CredencialesInvalidas);

            if (!user.Activo)
                return Result.Failure<LoginResponse>(IdentityErrors.UsuarioInactivo);

            var workstation = await _workstationRepository.GetByCodigoAndEmpresaAsync(
                command.CodigoEstacion, command.IdEmpresa, cancellationToken);

            if (workstation is null)
                return Result.Failure<LoginResponse>(IdentityErrors.EstacionNoEncontrada);

            if (!workstation.Activo)
                return Result.Failure<LoginResponse>(IdentityErrors.EstacionInactiva);

            var policies = await _userRepository.GetPoliciesAsync(user.Id, cancellationToken);

            var (token, expiresAt) = _jwtTokenService.GenerateToken(user, workstation, policies);

            return Result.Success(new LoginResponse(
                Token: token,
                ExpiresAt: expiresAt,
                IdUsuario: user.Id,
                Username: user.Username,
                IdTrabajador: user.IdTrabajador,
                NombreTrabajador: user.NombreTrabajador,
                IdEmpresa: user.IdEmpresa,
                IdSucursal: workstation.IdSucursal,
                NombreSucursal: workstation.NombreSucursal,
                IdEstacion: workstation.Id,
                CodigoEstacion: workstation.Codigo,
                Policies: policies));
        }

        /// <summary>
        /// Verifica la contraseña usando SHA-256.
        /// TODO: Ajustar al algoritmo real que usa el legacy (MD5, SHA1, BCrypt, etc.)
        /// </summary>
        private static bool VerifyPassword(string plainPassword, string storedHash)
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainPassword));
            var computedHash = Convert.ToHexString(hashBytes).ToLowerInvariant();
            return computedHash == storedHash.ToLowerInvariant();
        }
    }
}
