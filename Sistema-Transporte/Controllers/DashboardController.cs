using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaTransporte.Domain.Enums;
using SistemaTransporte.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Sistema_Transporte.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            var resumen = new
            {
                vehiculosDisponibles = await _context.Vehiculos
                    .CountAsync(v => v.Estado == EstadoVehiculo.Disponible),

                conductoresDisponibles = await _context.Conductores
                    .CountAsync(c => c.Estado == EstadoConductor.Disponible),

                solicitudesPendientes = await _context.SolicitudesTransporte
                    .CountAsync(s => s.Estado == EstadoSolicitud.Pendiente),

                viajesEnCurso = await _context.Viajes
                    .CountAsync(v => v.Estado == EstadoViaje.EnCurso),

                vehiculosEnMantenimiento = await _context.Vehiculos
                    .CountAsync(v => v.Estado == EstadoVehiculo.EnMantenimiento),

                consumosPendientes = await _context.ConsumosCombustible
                    .CountAsync(c => c.Estado == EstadoCombustible.Pendiente)
            };

            return Ok(resumen);
        }
    }
}