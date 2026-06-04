using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.DTOs.Viaje
{
    public class ActualizarViajeDto
    {
        public EstadoViaje Estado { get; set; }
        public string Observaciones { get; set; }
        public decimal DistanciaRecorrida { get; set; }
        public DateTime? FechaHoraLlegada { get; set; }
    }
}