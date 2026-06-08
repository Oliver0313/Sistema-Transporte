using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTransporte.Application.DTOs.Mantenimiento;
using SistemaTransporte.Application.Interfaces;

namespace Sistema_Transporte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MantenimientosController : ControllerBase
    {
        private readonly IMantenimientoService _mantenimientoService;

        public MantenimientosController(IMantenimientoService mantenimientoService)
        {
            _mantenimientoService = mantenimientoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var mantenimientos = await _mantenimientoService.GetAllAsync();
            return Ok(mantenimientos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var mantenimiento = await _mantenimientoService.GetByIdAsync(id);

            if (mantenimiento == null)
                return NotFound();

            return Ok(mantenimiento);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrearMantenimientoDto dto)
        {
            var mantenimiento = await _mantenimientoService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = mantenimiento.Id },
                mantenimiento);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarMantenimientoDto dto)
        {
            var actualizado = await _mantenimientoService.UpdateAsync(id, dto);

            if (!actualizado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _mantenimientoService.DeleteAsync(id);

            if (!eliminado)
                return NotFound();

            return NoContent();
        }
    }
}