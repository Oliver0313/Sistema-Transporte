namespace SistemaTransporte.Application.DTOs.Reportes
{
    public class SolicitudReporteDto
    {
        public int TotalSolicitudes { get; set; }
        public int Pendientes { get; set; }
        public int Aprobadas { get; set; }
        public int Rechazadas { get; set; }
        public int Canceladas { get; set; }
        public int Finalizadas { get; set; }
    }
}