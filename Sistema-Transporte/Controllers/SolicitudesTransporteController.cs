using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTransporte.Application.DTOs.SolicitudTransporte;
using SistemaTransporte.Application.Interfaces;

namespace Sistema_Transporte.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesTransporteController : ControllerBase
    {
        private readonly ISolicitudTransporteService _solicitudService;

        public SolicitudesTransporteController(ISolicitudTransporteService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var solicitudes = await _solicitudService.GetAllAsync();
            return Ok(solicitudes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var solicitud = await _solicitudService.GetByIdAsync(id);

            if (solicitud == null)
                return NotFound();

            return Ok(solicitud);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CrearSolicitudTransporteDto dto)
        {
            var solicitud = await _solicitudService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = solicitud.Id }, solicitud);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActualizarSolicitudTransporteDto dto)
        {
            var actualizado = await _solicitudService.UpdateAsync(id, dto);

            if (!actualizado)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _solicitudService.DeleteAsync(id);

            if (!eliminado)
                return NotFound();

            return NoContent();
        }
    }
}