using SistemaTransporte.Application.DTOs.Usuario;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Domain.Entities;

namespace SistemaTransporte.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IRepository<Usuario> _usuarioRepository;
        private readonly IRepository<Rol> _rolRepository;

        public UsuarioService(IRepository<Usuario> usuarioRepository, IRepository<Rol> rolRepository)
        {
            _usuarioRepository = usuarioRepository;
            _rolRepository = rolRepository;
        }

        public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var roles = await _rolRepository.GetAllAsync();

            return usuarios.Select(u =>
            {
                var rol = roles.FirstOrDefault(r => r.Id == u.RolId);

                return new UsuarioDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Correo = u.Correo,
                    RolId = u.RolId,
                    RolNombre = rol?.Nombre ?? "Sin rol",
                    EstadoUsuario = u.EstadoUsuario,
                    UltimoAcceso = u.UltimoAcceso
                };
            });
        }

        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                return null;

            var rol = await _rolRepository.GetByIdAsync(usuario.RolId);

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                RolId = usuario.RolId,
                RolNombre = rol?.Nombre ?? "Sin rol",
                EstadoUsuario = usuario.EstadoUsuario,
                UltimoAcceso = usuario.UltimoAcceso
            };
        }

        public async Task<bool> UpdateAsync(int id, ActualizarUsuarioDto dto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);

            if (usuario == null)
                return false;

            var rol = await _rolRepository.GetByIdAsync(dto.RolId);

            if (rol == null)
                throw new Exception("El rol no existe.");

            usuario.RolId = dto.RolId;
            usuario.EstadoUsuario = dto.EstadoUsuario;

            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return true;
        }
    }
}