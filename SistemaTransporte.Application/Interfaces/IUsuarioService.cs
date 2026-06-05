using SistemaTransporte.Application.DTOs.Usuario;

namespace SistemaTransporte.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDto>> GetAllAsync();
        Task<UsuarioDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, ActualizarUsuarioDto dto);
    }
}