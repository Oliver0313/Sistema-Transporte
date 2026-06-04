using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.DTOs.Viaje
{
    public class ViajeDto
    {
        public int Id { get; set; }
        public int AsignacionId { get; set; }
        public DateTime FechaHoraSalida { get; set; }
        public DateTime? FechaHoraLlegada { get; set; }
        public EstadoViaje Estado { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public int CantidadPasajeros { get; set; }
        public decimal DistanciaRecorrida { get; set; }
    }
}
