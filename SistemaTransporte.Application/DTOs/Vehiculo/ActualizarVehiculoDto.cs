using SistemaTransporte.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Application.DTOs.Vehiculo
{
    public class ActualizarVehiculoDto
    {
        public string Matricula { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Anio { get; set; }
        public string Color { get; set; }
        public string? ImagenUrl { get; set; }
        public string Tipo { get; set; }
        public int Capacidad { get; set; }
        public int Kilometraje { get; set; }
        public EstadoVehiculo Estado { get; set; }
    }
}
