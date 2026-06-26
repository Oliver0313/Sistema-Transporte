using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SistemaTransporte.Application.DTOs.Auth;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<Usuario> _usuarioRepository;
        private readonly IRepository<Rol> _rolRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(
            IRepository<Usuario> usuarioRepository,
            IRepository<Rol> rolRepository,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _rolRepository = rolRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            if (usuarios.Any(u => u.Correo == dto.Correo))
                throw new Exception("El correo ya está registrado.");

            var rol = await _rolRepository.GetByIdAsync(dto.RolId);

            if (rol == null)
                throw new Exception("El rol no existe.");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Correo = dto.Correo,
                ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                RolId = dto.RolId,
                Rol = rol,
                EstadoUsuario = EstadoUsuario.Activo
            };

            await _usuarioRepository.AddAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();

            var token = GenerarToken(usuario, rol.Nombre);

            return new AuthResponseDto
            {
                Token = token,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = rol.Nombre
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            var usuario = usuarios.FirstOrDefault(u => u.Correo == dto.Correo);

            if (usuario == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Contrasena, usuario.ContrasenaHash))
                return null;

            // === CONTROL DE USUARIO INACTIVO ===
            if (usuario.EstadoUsuario == EstadoUsuario.Inactivo)
            {
                throw new UnauthorizedAccessException("Su cuenta está inactiva. Contacte al administrador.");
            }

            var rol = await _rolRepository.GetByIdAsync(usuario.RolId);

            if (rol == null)
                return null;

            usuario.UltimoAcceso = DateTime.Now;
            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            var token = GenerarToken(usuario, rol.Nombre);

            return new AuthResponseDto
            {
                Token = token,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Rol = rol.Nombre
            };
        }

        public async Task<object> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            var usuario = usuarios.FirstOrDefault(u => u.Correo == dto.Correo);

            if (usuario == null)
            {
                return new
                {
                    mensaje = "Si el correo existe, se enviarán instrucciones de recuperación."
                };
            }

            var token = Guid.NewGuid().ToString();

            usuario.ResetPasswordToken = token;
            usuario.ResetPasswordTokenExpira = DateTime.Now.AddMinutes(30);

            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            var resetLink = $"http://localhost:5173/reset-password?token={token}";

            var subject = "Recuperación de contraseña - Sistema Transporte";

            var body = $@"
                <h2>Recuperación de contraseña</h2>
                <p>Hola {usuario.Nombre},</p>
                <p>Recibimos una solicitud para restablecer tu contraseña.</p>
                <p>Haz clic en el siguiente enlace para continuar:</p>
                <p>
                    <a href='{resetLink}'>Restablecer contraseña</a>
                </p>
                <p>Este enlace expirará en 30 minutos.</p>
                <p>Si no solicitaste este cambio, puedes ignorar este mensaje.</p>
            ";

            await _emailService.SendEmailAsync(usuario.Correo, subject, body);

            return new
            {
                mensaje = "Se enviaron instrucciones de recuperación al correo registrado."
            };
        }

        public async Task<object?> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var usuarios = await _usuarioRepository.GetAllAsync();

            var usuario = usuarios.FirstOrDefault(u =>
                u.ResetPasswordToken == dto.Token &&
                u.ResetPasswordTokenExpira > DateTime.Now
            );

            if (usuario == null)
                return null;

            usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(dto.NuevaContrasena);
            usuario.ResetPasswordToken = null;
            usuario.ResetPasswordTokenExpira = null;

            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return new
            {
                mensaje = "Contraseña actualizada correctamente."
            };
        }

        private string GenerarToken(Usuario usuario, string rol)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, rol)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}