using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTransporte.Application.DTOs.Usuario;
using SistemaTransporte.Application.Interfaces;

namespace Sistema_Transporte.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);

            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarUsuarioDto dto)
        {
            var actualizado = await _usuarioService.UpdateAsync(id, dto);

            if (!actualizado)
                return NotFound();

            return NoContent();
        }
    }
}