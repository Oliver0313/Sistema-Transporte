using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTransporte.Application.DTOs.Conductor;
using SistemaTransporte.Application.Interfaces;

namespace Sistema_Transporte.Controllers
{
    [Authorize] 
    [ApiController]
    [Route("api/[controller]")]
    public class ConductoresController : ControllerBase
    {
        private readonly IConductorService _conductorService;

        public ConductoresController(IConductorService conductorService)
        {
            _conductorService = conductorService;
        }

        [HttpGet] 
        public async Task<IActionResult> GetAll()
        {
            var conductores = await _conductorService.GetAllAsync();
            return Ok(conductores);
        }

        [HttpGet("{id}")] 
        public async Task<IActionResult> GetById(int id)
        {
            var conductor = await _conductorService.GetByIdAsync(id);

            if (conductor == null)
                return NotFound();

            return Ok(conductor);
        }

        [HttpPost] 
        [Authorize(Roles = "SuperAdmin,Administrador")]
        public async Task<IActionResult> Create(CrearConductorDto dto)
        {
            var conductor = await _conductorService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = conductor.Id }, conductor);
        }

        [HttpPut("{id}")] 
        [Authorize(Roles = "SuperAdmin,Administrador")]
        public async Task<IActionResult> Update(int id, ActualizarConductorDto dto)
        {
            var actualizado = await _conductorService.UpdateAsync(id, dto);

            if (!actualizado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")] 
        [Authorize(Roles = "SuperAdmin,Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            return BadRequest(new
            {
                message = "Por políticas de auditoría, los conductores no pueden ser eliminados permanentemente del sistema."
            });
        }
    }
}