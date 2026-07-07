using SistemaTransporte.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Application.DTOs.Mantenimiento
{
    public class ActualizarMantenimientoDto
    {
        public DateTime FechaMantenimiento { get; set; }
        public EstadoMantenimiento Estado { get; set; }
        public TipoMantenimiento TipoMantenimiento { get; set; }
        public string Descripcion { get; set; }
        public decimal Costo { get; set; }
        public string Taller { get; set; }
        public DateTime? ProximoMantenimiento { get; set; }
    }
}
