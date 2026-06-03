using SistemaTransporte.Application.DTOs.Vehiculo;

namespace SistemaTransporte.Application.Interfaces
{
    public interface IVehiculoService
    {
        Task<IEnumerable<VehiculoDto>> GetAllAsync();
        Task<VehiculoDto?> GetByIdAsync(int id);
        Task<VehiculoDto> CreateAsync(CrearVehiculoDto dto);
        Task<bool> UpdateAsync(int id, ActualizarVehiculoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
