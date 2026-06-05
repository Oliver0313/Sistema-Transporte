using SistemaTransporte.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Application.DTOs.Conductor
{
    public class ActualizarConductorDto
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cedula { get; set; }
        public string Email { get; set; }
        public string Licencia { get; set; }
        public TipoLicencia TipoLicencia { get; set; }
        public DateTime? FechaVencimientoLicencia { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public int SupervisorId { get; set; }
        public EstadoConductor Estado { get; set; }
    }
}
