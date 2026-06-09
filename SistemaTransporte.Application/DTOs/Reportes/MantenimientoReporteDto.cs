namespace SistemaTransporte.Application.DTOs.Reportes
{
    public class MantenimientoReporteDto
    {
        public int TotalMantenimientos { get; set; }
        public int Programados { get; set; }
        public int Pendientes { get; set; }
        public int EnProgreso { get; set; }
        public int Completados { get; set; }
        public int Vencidos { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal CostoPromedio { get; set; }
    }
}