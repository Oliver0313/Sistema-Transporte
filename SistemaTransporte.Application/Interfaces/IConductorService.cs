using SistemaTransporte.Application.DTOs.Conductor;

namespace SistemaTransporte.Application.Interfaces
{
    public interface IConductorService
    {
        Task<IEnumerable<ConductorDto>> GetAllAsync();
        Task<ConductorDto?> GetByIdAsync(int id);
        Task<ConductorDto> CreateAsync(CrearConductorDto dto);
        Task<bool> UpdateAsync(int id, ActualizarConductorDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
