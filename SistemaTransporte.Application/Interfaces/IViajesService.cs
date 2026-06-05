using SistemaTransporte.Application.DTOs.Viaje;

namespace SistemaTransporte.Application.Interfaces
{
    public interface IViajeService
    {
        Task<IEnumerable<ViajeDto>> GetAllAsync();
        Task<ViajeDto?> GetByIdAsync(int id);
        Task<ViajeDto> CreateAsync(CrearViajeDto dto);
        Task<bool> UpdateAsync(int id, ActualizarViajeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}