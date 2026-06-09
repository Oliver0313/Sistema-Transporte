namespace SistemaTransporte.Application.DTOs.Reportes
{
    public class CombustibleReporteDto
    {
        public int TotalRegistros { get; set; }
        public decimal TotalGalones { get; set; }
        public decimal TotalGastado { get; set; }
        public decimal TotalKilometros { get; set; }
        public int Pendientes { get; set; }
        public int Aprobados { get; set; }
        public int Rechazados { get; set; }
        public decimal CostoPromedioPorGalon { get; set; }
        public decimal RendimientoKmPorGalon { get; set; }
    }
}