using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTransporte.Domain.Enums;
using SistemaTransporte.Infrastructure.Data;

namespace Sistema_Transporte.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("vehiculos")]
        public async Task<IActionResult> ReporteVehiculos()
        {
            return Ok(new
            {
                totalVehiculos = await _context.Vehiculos.CountAsync(),
                disponibles = await _context.Vehiculos.CountAsync(v => v.Estado == EstadoVehiculo.Disponible),
                enViaje = await _context.Vehiculos.CountAsync(v => v.Estado == EstadoVehiculo.EnViaje),
                enMantenimiento = await _context.Vehiculos.CountAsync(v => v.Estado == EstadoVehiculo.EnMantenimiento),
                fueraDeServicio = await _context.Vehiculos.CountAsync(v => v.Estado == EstadoVehiculo.FueraDeServicio)
            });
        }

        [HttpGet("conductores")]
        public async Task<IActionResult> ReporteConductores()
        {
            return Ok(new
            {
                totalConductores = await _context.Conductores.CountAsync(),
                disponibles = await _context.Conductores.CountAsync(c => c.Estado == EstadoConductor.Disponible),
                enViaje = await _context.Conductores.CountAsync(c => c.Estado == EstadoConductor.EnViaje),
                suspendidos = await _context.Conductores.CountAsync(c => c.Estado == EstadoConductor.Suspendido),
                inactivos = await _context.Conductores.CountAsync(c => c.Estado == EstadoConductor.Inactivo)
            });
        }

        [HttpGet("solicitudes")]
        public async Task<IActionResult> ReporteSolicitudes()
        {
            return Ok(new
            {
                totalSolicitudes = await _context.SolicitudesTransporte.CountAsync(),
                pendientes = await _context.SolicitudesTransporte.CountAsync(s => s.Estado == EstadoSolicitud.Pendiente),
                aprobadas = await _context.SolicitudesTransporte.CountAsync(s => s.Estado == EstadoSolicitud.Aprobada),
                rechazadas = await _context.SolicitudesTransporte.CountAsync(s => s.Estado == EstadoSolicitud.Rechazada),
                canceladas = await _context.SolicitudesTransporte.CountAsync(s => s.Estado == EstadoSolicitud.Cancelada),
                finalizadas = await _context.SolicitudesTransporte.CountAsync(s => s.Estado == EstadoSolicitud.Finalizada)
            });
        }

        [HttpGet("viajes")]
        public async Task<IActionResult> ReporteViajes()
        {
            return Ok(new
            {
                totalViajes = await _context.Viajes.CountAsync(),
                programados = await _context.Viajes.CountAsync(v => v.Estado == EstadoViaje.Programado),
                enCurso = await _context.Viajes.CountAsync(v => v.Estado == EstadoViaje.EnCurso),
                finalizados = await _context.Viajes.CountAsync(v => v.Estado == EstadoViaje.Finalizado),
                cancelados = await _context.Viajes.CountAsync(v => v.Estado == EstadoViaje.Cancelado),
                totalPasajeros = await _context.Viajes.SumAsync(v => v.CantidadPasajeros),
                distanciaTotal = await _context.Viajes.SumAsync(v => v.DistanciaRecorrida)
            });
        }

        [HttpGet("mantenimientos")]
        public async Task<IActionResult> ReporteMantenimientos()
        {
            return Ok(new
            {
                totalMantenimientos = await _context.Mantenimientos.CountAsync(),
                programados = await _context.Mantenimientos.CountAsync(m => m.Estado == EstadoMantenimiento.Programado),
                pendientes = await _context.Mantenimientos.CountAsync(m => m.Estado == EstadoMantenimiento.Pendiente),
                enProgreso = await _context.Mantenimientos.CountAsync(m => m.Estado == EstadoMantenimiento.EnProgreso),
                completados = await _context.Mantenimientos.CountAsync(m => m.Estado == EstadoMantenimiento.Completado),
                vencidos = await _context.Mantenimientos.CountAsync(m => m.Estado == EstadoMantenimiento.Vencido),
                costoTotal = await _context.Mantenimientos.SumAsync(m => m.Costo)
            });
        }

        [HttpGet("combustible")]
        public async Task<IActionResult> ReporteCombustible()
        {
            return Ok(new
            {
                totalRegistros = await _context.ConsumosCombustible.CountAsync(),
                totalGalones = await _context.ConsumosCombustible.SumAsync(c => c.Galones),
                totalGastado = await _context.ConsumosCombustible.SumAsync(c => c.Costo),
                totalKilometros = await _context.ConsumosCombustible.SumAsync(c => c.KilometrosRecorridos),
                pendientes = await _context.ConsumosCombustible.CountAsync(c => c.Estado == EstadoCombustible.Pendiente),
                aprobados = await _context.ConsumosCombustible.CountAsync(c => c.Estado == EstadoCombustible.Aprobado),
                rechazados = await _context.ConsumosCombustible.CountAsync(c => c.Estado == EstadoCombustible.Rechazado)
            });
        }
    }
}