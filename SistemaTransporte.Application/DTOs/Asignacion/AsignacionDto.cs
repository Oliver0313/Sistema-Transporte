using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.DTOs.Asignacion
{
    public class AsignacionDto
    {
        public int Id { get; set; }
        public int SolicitudTransporteId { get; set; }
        public int ConductorId { get; set; }
        public int VehiculoId { get; set; }
        public DateTime FechaHoraAsignacion { get; set; }
        public EstadoAsignacion Estado { get; set; }
        public int UsuarioAsignadorId { get; set; }
    }
}
