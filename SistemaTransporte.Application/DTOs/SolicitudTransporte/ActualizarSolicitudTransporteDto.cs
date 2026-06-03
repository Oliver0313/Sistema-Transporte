using SistemaTransporte.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Application.DTOs.SolicitudTransporte
{
    public class ActualizarSolicitudTransporteDto
    {
        public string AreaSolicitante { get; set; }
        public int CantidadColaboradores { get; set; }
        public DateTime HoraSalida { get; set; }
        public DateTime HoraRegreso { get; set; }
        public string Destino { get; set; }
        public string Motivo { get; set; }
        public EstadoSolicitud Estado { get; set; }
    }
}
