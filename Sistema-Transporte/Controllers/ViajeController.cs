using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTransporte.Application.DTOs.Viaje;
using SistemaTransporte.Application.Interfaces;

namespace Sistema_Transporte.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class ViajesController : ControllerBase
    {
        private readonly IViajeService _viajeService;

        public ViajesController(IViajeService viajeService)
        {
            _viajeService = viajeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var viajes = await _viajeService.GetAllAsync();
            return Ok(viajes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var viaje = await _viajeService.GetByIdAsync(id);

            if (viaje == null)
                return NotFound();

            return Ok(viaje);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrearViajeDto dto)
        {
            var viaje = await _viajeService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = viaje.Id },
                viaje);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] ActualizarViajeDto dto)
        {
            var actualizado = await _viajeService.UpdateAsync(id, dto);

            if (!actualizado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _viajeService.DeleteAsync(id);

            if (!eliminado)
                return NotFound();

            return NoContent();
        }
    }
}