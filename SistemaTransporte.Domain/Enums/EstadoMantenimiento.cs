using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Domain.Enums
{
    public enum EstadoMantenimiento
    {
        Programado = 1,
        Pendiente = 2,
        EnProgreso = 3,
        Completado = 4,
        Vencido = 5
    }
}
