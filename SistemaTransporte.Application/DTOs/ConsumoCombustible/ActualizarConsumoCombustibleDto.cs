using SistemaTransporte.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Application.DTOs.ConsumoCombustible
{
    public class ActualizarConsumoCombustibleDto
    {
        public decimal Galones { get; set; }
        public decimal Costo { get; set; }
        public decimal KilometrosRecorridos { get; set; }
        public EstadoCombustible Estado { get; set; }
    }
}
