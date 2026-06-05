using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Domain.Entities
{
    public class Asignacion
    {
        public int Id { get; set; }
        public int SolicitudTransporteId { get; set; }
        public int ConductorId { get; set; }
        public int VehiculoId { get; set; }
        public DateTime FechaHoraAsignacion { get; set; }
        public int UsuarioAsignadorId { get; set; }
        public EstadoAsignacion Estado { get; set; }
    }
}
