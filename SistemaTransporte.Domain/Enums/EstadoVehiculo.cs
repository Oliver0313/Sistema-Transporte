using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Domain.Enums
{
    public enum EstadoVehiculo
    {
        Disponible = 1,
        EnViaje = 2,
        EnMantenimiento = 3,
        FueraDeServicio = 4
    }
}
