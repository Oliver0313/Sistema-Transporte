namespace SistemaTransporte.Application.DTOs.Auth;

public class ResetPasswordDto
{
    public string Token { get; set; } = string.Empty;
    public string NuevaContrasena { get; set; } = string.Empty;
}