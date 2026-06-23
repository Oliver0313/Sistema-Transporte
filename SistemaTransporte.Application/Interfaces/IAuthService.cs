using SistemaTransporte.Application.DTOs.Auth;

namespace SistemaTransporte.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<object> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<object?> ResetPasswordAsync(ResetPasswordDto dto);
    }
}