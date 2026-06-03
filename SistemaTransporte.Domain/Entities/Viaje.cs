using SistemaTransporte.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Domain.Entities
{
    public class Viaje
    {
        public int Id { get; set; }
        public int AsignacionId { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public DateTime FechaHoraLlegada { get; set; }
        public EstadoViaje Estado { get; set; }
        public string Observaciones { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public int CantidadPasajeros { get; set; }
        public decimal DistanciaRecorrida { get; set; }
    }
}
