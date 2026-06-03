using SistemaTransporte.Application.DTOs.Asignacion;

namespace SistemaTransporte.Application.Interfaces
{
    public interface IAsignacionService
    {
        Task<IEnumerable<AsignacionDto>> GetAllAsync();
        Task<AsignacionDto?> GetByIdAsync(int id);
        Task<AsignacionDto> CreateAsync(CrearAsignacionDto dto);
        Task<bool> UpdateAsync(int id, ActualizarAsignacionDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
