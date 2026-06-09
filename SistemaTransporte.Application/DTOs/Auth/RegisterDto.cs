using System.ComponentModel.DataAnnotations;

namespace SistemaTransporte.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Contrasena { get; set; } = string.Empty;

        [Required]
        public int RolId { get; set; }
    }
}