using SistemaTransporte.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Application.DTOs.ConsumoCombustible
{
    public class ConsumoCombustibleDto
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public int ConductorId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Galones { get; set; }
        public decimal Costo { get; set; }
        public decimal KilometrosRecorridos { get; set; }
        public EstadoCombustible Estado { get; set; }
    }
}
