using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Application.DTOs.Asignacion
{
    public class CrearAsignacionDto
    {
        public int SolicitudTransporteId { get; set; }
        public int ConductorId { get; set; }
        public int VehiculoId { get; set; }
        public int UsuarioAsignadorId { get; set; }
    }
}
