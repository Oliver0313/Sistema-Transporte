using SistemaTransporte.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public EstadoUsuario EstadoUsuario { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string ContrasenaHash { get; set; }
        public Rol Rol { get; set; }
        public int RolId { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }
}
