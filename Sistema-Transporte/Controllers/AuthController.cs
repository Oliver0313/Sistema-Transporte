using Microsoft.AspNetCore.Mvc;
using SistemaTransporte.Application.DTOs.Auth;
using SistemaTransporte.Application.Interfaces;

namespace Sistema_Transporte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var response = await _authService.RegisterAsync(dto);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);

            if (response == null)
                return Unauthorized("Correo o contraseña incorrectos.");

            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var response = await _authService.ForgotPasswordAsync(dto);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var response = await _authService.ResetPasswordAsync(dto);

            if (response == null)
                return BadRequest(new { mensaje = "Token inválido o expirado." });

            return Ok(response);
        }
    }
}