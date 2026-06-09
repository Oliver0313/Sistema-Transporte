using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTransporte.Application.DTOs.Reportes;
using SistemaTransporte.Domain.Enums;
using SistemaTransporte.Infrastructure.Data;

namespace Sistema_Transporte.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrador")]
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
            var reporte = new VehiculoReporteDto
            {
                TotalVehiculos = await _context.Vehiculos.CountAsync(),
                Disponibles = await _context.Vehiculos.CountAsync(v => v.Estado == EstadoVehiculo.Disponible),
                EnViaje = await _context.Vehiculos.CountAsync(v => v.Estado == EstadoVehiculo.EnViaje),
                EnMantenimiento = await _context.Vehiculos.CountAsync(v => v.Estado == EstadoVehiculo.EnMantenimiento),
                FueraDeServicio = await _context.Vehiculos.CountAsync(v => v.Estado == EstadoVehiculo.FueraDeServicio)
            };

            return Ok(reporte);
        }

        [HttpGet("conductores")]
        public async Task<IActionResult> ReporteConductores()
        {
            var reporte = new ConductorReporteDto
            {
                TotalConductores = await _context.Conductores.CountAsync(),
                Disponibles = await _context.Conductores.CountAsync(c => c.Estado == EstadoConductor.Disponible),
                EnViaje = await _context.Conductores.CountAsync(c => c.Estado == EstadoConductor.EnViaje),
                Suspendidos = await _context.Conductores.CountAsync(c => c.Estado == EstadoConductor.Suspendido),
                Inactivos = await _context.Conductores.CountAsync(c => c.Estado == EstadoConductor.Inactivo)
            };

            return Ok(reporte);
        }

        [HttpGet("solicitudes")]
        public async Task<IActionResult> ReporteSolicitudes()
        {
            var reporte = new SolicitudReporteDto
            {
                TotalSolicitudes = await _context.SolicitudesTransporte.CountAsync(),
                Pendientes = await _context.SolicitudesTransporte.CountAsync(s => s.Estado == EstadoSolicitud.Pendiente),
                Aprobadas = await _context.SolicitudesTransporte.CountAsync(s => s.Estado == EstadoSolicitud.Aprobada),
                Rechazadas = await _context.SolicitudesTransporte.CountAsync(s => s.Estado == EstadoSolicitud.Rechazada),
                Canceladas = await _context.SolicitudesTransporte.CountAsync(s => s.Estado == EstadoSolicitud.Cancelada),
                Finalizadas = await _context.SolicitudesTransporte.CountAsync(s => s.Estado == EstadoSolicitud.Finalizada)
            };

            return Ok(reporte);
        }

        [HttpGet("viajes")]
        public async Task<IActionResult> ReporteViajes()
        {
            var totalViajes = await _context.Viajes.CountAsync();
            var finalizados = await _context.Viajes.CountAsync(v => v.Estado == EstadoViaje.Finalizado);

            var reporte = new ViajeReporteDto
            {
                TotalViajes = totalViajes,
                Programados = await _context.Viajes.CountAsync(v => v.Estado == EstadoViaje.Programado),
                EnCurso = await _context.Viajes.CountAsync(v => v.Estado == EstadoViaje.EnCurso),
                Finalizados = finalizados,
                Cancelados = await _context.Viajes.CountAsync(v => v.Estado == EstadoViaje.Cancelado),
                TotalPasajeros = await _context.Viajes.SumAsync(v => v.CantidadPasajeros),
                DistanciaTotal = await _context.Viajes.SumAsync(v => v.DistanciaRecorrida),
                PorcentajeFinalizados = totalViajes == 0 ? 0 : finalizados * 100m / totalViajes
            };

            return Ok(reporte);
        }

        [HttpGet("mantenimientos")]
        public async Task<IActionResult> ReporteMantenimientos()
        {
            var totalMantenimientos = await _context.Mantenimientos.CountAsync();
            var costoTotal = await _context.Mantenimientos.SumAsync(m => m.Costo);

            var reporte = new MantenimientoReporteDto
            {
                TotalMantenimientos = totalMantenimientos,
                Programados = await _context.Mantenimientos.CountAsync(m => m.Estado == EstadoMantenimiento.Programado),
                Pendientes = await _context.Mantenimientos.CountAsync(m => m.Estado == EstadoMantenimiento.Pendiente),
                EnProgreso = await _context.Mantenimientos.CountAsync(m => m.Estado == EstadoMantenimiento.EnProgreso),
                Completados = await _context.Mantenimientos.CountAsync(m => m.Estado == EstadoMantenimiento.Completado),
                Vencidos = await _context.Mantenimientos.CountAsync(m => m.Estado == EstadoMantenimiento.Vencido),
                CostoTotal = costoTotal,
                CostoPromedio = totalMantenimientos == 0 ? 0 : costoTotal / totalMantenimientos
            };

            return Ok(reporte);
        }

        [HttpGet("combustible")]
        public async Task<IActionResult> ReporteCombustible()
        {
            var totalGalones = await _context.ConsumosCombustible.SumAsync(c => c.Galones);
            var totalGastado = await _context.ConsumosCombustible.SumAsync(c => c.Costo);
            var totalKilometros = await _context.ConsumosCombustible.SumAsync(c => c.KilometrosRecorridos);

            var reporte = new CombustibleReporteDto
            {
                TotalRegistros = await _context.ConsumosCombustible.CountAsync(),
                TotalGalones = totalGalones,
                TotalGastado = totalGastado,
                TotalKilometros = totalKilometros,
                Pendientes = await _context.ConsumosCombustible.CountAsync(c => c.Estado == EstadoCombustible.Pendiente),
                Aprobados = await _context.ConsumosCombustible.CountAsync(c => c.Estado == EstadoCombustible.Aprobado),
                Rechazados = await _context.ConsumosCombustible.CountAsync(c => c.Estado == EstadoCombustible.Rechazado),
                CostoPromedioPorGalon = totalGalones == 0 ? 0 : totalGastado / totalGalones,
                RendimientoKmPorGalon = totalGalones == 0 ? 0 : totalKilometros / totalGalones
            };

            return Ok(reporte);
        }
    }
}