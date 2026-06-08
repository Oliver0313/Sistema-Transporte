using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTransporte.Application.DTOs.ConsumoCombustible;
using SistemaTransporte.Application.Interfaces;

namespace Sistema_Transporte.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumosCombustibleController : ControllerBase
    {
        private readonly IConsumoCombustibleService _consumoService;

        public ConsumosCombustibleController(IConsumoCombustibleService consumoService)
        {
            _consumoService = consumoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var consumos = await _consumoService.GetAllAsync();
            return Ok(consumos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var consumo = await _consumoService.GetByIdAsync(id);

            if (consumo == null)
                return NotFound();

            return Ok(consumo);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrearConsumoCombustibleDto dto)
        {
            var consumo = await _consumoService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = consumo.Id }, consumo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarConsumoCombustibleDto dto)
        {
            var actualizado = await _consumoService.UpdateAsync(id, dto);

            if (!actualizado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _consumoService.DeleteAsync(id);

            if (!eliminado)
                return NotFound();

            return NoContent();
        }
    }
}
