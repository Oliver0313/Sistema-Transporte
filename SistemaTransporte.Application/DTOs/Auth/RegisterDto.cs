namespace SistemaTransporte.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public int RolId { get; set; }
    }
}