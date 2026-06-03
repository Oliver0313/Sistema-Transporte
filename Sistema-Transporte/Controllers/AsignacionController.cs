using Microsoft.AspNetCore.Mvc;
using SistemaTransporte.Application.DTOs.Asignacion;
using SistemaTransporte.Application.Interfaces;

namespace Sistema_Transporte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionesController : ControllerBase
    {
        private readonly IAsignacionService _asignacionService;

        public AsignacionesController(IAsignacionService asignacionService)
        {
            _asignacionService = asignacionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var asignaciones = await _asignacionService.GetAllAsync();
            return Ok(asignaciones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var asignacion = await _asignacionService.GetByIdAsync(id);

            if (asignacion == null)
                return NotFound();

            return Ok(asignacion);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrearAsignacionDto dto)
        {
            var asignacion = await _asignacionService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = asignacion.Id },
                asignacion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarAsignacionDto dto)
        {
            var actualizado = await _asignacionService.UpdateAsync(id, dto);

            if (!actualizado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _asignacionService.DeleteAsync(id);

            if (!eliminado)
                return NotFound();

            return NoContent();
        }
    }
}
