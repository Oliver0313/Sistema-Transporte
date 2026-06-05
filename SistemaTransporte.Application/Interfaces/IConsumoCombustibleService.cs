using SistemaTransporte.Application.DTOs.ConsumoCombustible;

namespace SistemaTransporte.Application.Interfaces
{
    public interface IConsumoCombustibleService
    {
        Task<IEnumerable<ConsumoCombustibleDto>> GetAllAsync();
        Task<ConsumoCombustibleDto?> GetByIdAsync(int id);
        Task<ConsumoCombustibleDto> CreateAsync(CrearConsumoCombustibleDto dto);
        Task<bool> UpdateAsync(int id, ActualizarConsumoCombustibleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
