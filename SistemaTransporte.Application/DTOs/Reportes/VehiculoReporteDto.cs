namespace SistemaTransporte.Application.DTOs.Reportes
{
    public class VehiculoReporteDto
    {
        public int TotalVehiculos { get; set; }
        public int Disponibles { get; set; }
        public int EnViaje { get; set; }
        public int EnMantenimiento { get; set; }
        public int FueraDeServicio { get; set; }
    }
}
