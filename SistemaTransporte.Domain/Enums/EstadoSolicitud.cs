using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaTransporte.Domain.Enums
{
    public enum EstadoSolicitud
    {
        Pendiente = 1,
        Aceptada = 2,
        Rechazada = 3,
        Cancelada = 4,
        Finalizada = 5
    }
}
