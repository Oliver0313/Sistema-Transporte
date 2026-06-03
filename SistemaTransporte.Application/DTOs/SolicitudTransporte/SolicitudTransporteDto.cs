using SistemaTransporte.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Application.DTOs.SolicitudTransporte
{
    public class SolicitudTransporteDto
    {
        public int Id { get; set; }
        public string AreaSolicitante { get; set; }
        public int CantidadColaboradores { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public DateTime FechaHoraRegreso { get; set; }
        public string Destino { get; set; }
        public string Motivo { get; set; }
        public EstadoSolicitud Estado { get; set; }
        public int UsuarioSolicitanteId { get; set; }
    }
}
