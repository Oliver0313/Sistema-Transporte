using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.DTOs.Vehiculo
{
    public class VehiculoDto
    {
        public int Id { get; set; }
        public string Matricula { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; } 
        public int Anio { get; set; }
        public string Color { get; set; }
        public int Capacidad { get; set; }
        public EstadoVehiculo Estado { get; set; }
        public DateTime? FechaUltimoMantenimiento { get; set; }
        public int Kilometraje { get; set; }
        public string Tipo { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
