using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.DTOs.Asignacion
{
    public class ActualizarAsignacionDto
    {
        public int VehiculoId { get; set; }
        public int ConductorId { get; set; }
        public EstadoAsignacion Estado { get; set; }
    }
}
