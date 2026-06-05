namespace SistemaTransporte.Application.DTOs.Viaje
{
    public class CrearViajeDto
    {
        public int AsignacionId { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public int CantidadPasajeros { get; set; }
    }
}