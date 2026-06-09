namespace SistemaTransporte.Application.DTOs.Reportes
{
    public class ViajeReporteDto
    {
        public int TotalViajes { get; set; }
        public int Programados { get; set; }
        public int EnCurso { get; set; }
        public int Finalizados { get; set; }
        public int Cancelados { get; set; }
        public int TotalPasajeros { get; set; }
        public decimal DistanciaTotal { get; set; }
        public decimal PorcentajeFinalizados { get; set; }
    }
}