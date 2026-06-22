namespace SistemaTransporte.Application.DTOs.Reportes
{
    public class ConductorReporteDto
    {
        public int TotalConductores { get; set; }
        public int Disponibles { get; set; }
        public int EnViaje { get; set; }
        public int Suspendidos { get; set; }
        public int Inactivos { get; set; }
    }
}