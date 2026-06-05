using SistemaTransporte.Application.DTOs.Mantenimiento;

namespace SistemaTransporte.Application.Interfaces
{
    public interface IMantenimientoService
    {
        Task<IEnumerable<MantenimientoDto>> GetAllAsync();
        Task<MantenimientoDto?> GetByIdAsync(int id);
        Task<MantenimientoDto> CreateAsync(CrearMantenimientoDto dto);
        Task<bool> UpdateAsync(int id, ActualizarMantenimientoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}