using SistemaTransporte.Application.DTOs.SolicitudTransporte;

namespace SistemaTransporte.Application.Interfaces
{
    public interface ISolicitudTransporteService
    {
        Task<IEnumerable<SolicitudTransporteDto>> GetAllAsync();

        Task<SolicitudTransporteDto?> GetByIdAsync(int id);

        Task<SolicitudTransporteDto> CreateAsync(CrearSolicitudTransporteDto dto);

        Task<bool> UpdateAsync(int id, ActualizarSolicitudTransporteDto dto);

        Task<bool> DeleteAsync(int id);
    }
}