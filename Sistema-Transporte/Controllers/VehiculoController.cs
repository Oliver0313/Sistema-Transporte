using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTransporte.Application.DTOs.Vehiculo;
using SistemaTransporte.Application.Interfaces;

namespace Sistema_Transporte.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculosController : ControllerBase
    {
        private readonly IVehiculoService _vehiculoService;

        public VehiculosController(IVehiculoService vehiculoService)
        {
            _vehiculoService = vehiculoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vehiculos = await _vehiculoService.GetAllAsync();
            return Ok(vehiculos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vehiculo = await _vehiculoService.GetByIdAsync(id);

            if (vehiculo == null)
                return NotFound();

            return Ok(vehiculo);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrearVehiculoDto dto)
        {
            var vehiculo = await _vehiculoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = vehiculo.Id }, vehiculo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarVehiculoDto dto)
        {
            var actualizado = await _vehiculoService.UpdateAsync(id, dto);

            if (!actualizado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _vehiculoService.DeleteAsync(id);

            if (!eliminado)
                return NotFound();

            return NoContent();
        }
    }
}