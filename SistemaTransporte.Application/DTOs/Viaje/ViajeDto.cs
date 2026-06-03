using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Application.DTOs.Viaje
{
    public class ViajeDto
    {
        public int Id { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public DateTime? FechaHoraLlegada { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public int AsignacionId { get; set; }
        public string Observaciones { get; set; }
    }
}
