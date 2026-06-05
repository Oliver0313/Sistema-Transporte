using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.DTOs.Usuario
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public int RolId { get; set; }
        public string RolNombre { get; set; } = string.Empty;
        public EstadoUsuario EstadoUsuario { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }
}