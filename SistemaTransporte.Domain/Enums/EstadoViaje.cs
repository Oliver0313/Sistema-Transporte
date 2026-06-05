using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Domain.Enums
{
    public enum EstadoViaje
    {
        Programado = 1,
        EnCurso = 2,
        Finalizado = 3,
        Cancelado = 4
    }
}
