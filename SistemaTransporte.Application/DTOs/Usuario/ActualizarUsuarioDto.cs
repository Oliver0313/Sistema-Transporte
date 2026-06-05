using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.DTOs.Usuario
{
    public class ActualizarUsuarioDto
    {
        public int RolId { get; set; }
        public EstadoUsuario EstadoUsuario { get; set; }
    }
}