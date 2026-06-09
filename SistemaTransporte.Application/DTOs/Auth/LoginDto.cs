using System.ComponentModel.DataAnnotations;

namespace SistemaTransporte.Application.DTOs.Auth
{
    public class LoginDto
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}