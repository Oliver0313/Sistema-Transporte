using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTransporte.Application.DTOs.Reportes;
using SistemaTransporte.Domain.Enums;
using SistemaTransporte.Infrastructure.Data;

namespace Sistema_Transporte.Controllers
{
    // 🔒 Se agregaron los roles Supervisor y Operador para permitirles la consulta de datos
    [Authorize(Roles = "SuperAdmin,Administrador,Supervisor,Operador")]
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

        [HttpGet("viajes-por-mes")]
        public async Task<IActionResult> GetViajesPorMes([FromQuery] int anio)
        {
            var viajes = await _context.Viajes
                .Where(v => v.FechaHoraSalida.Year == anio)
                .GroupBy(v => v.FechaHoraSalida.Month)
                .Select(g => new
                {
                    Mes = g.Key == 1 ? "Ene" : g.Key == 2 ? "Feb" : g.Key == 3 ? "Mar" : g.Key == 4 ? "Abr" : g.Key == 5 ? "May" : g.Key == 6 ? "Jun" : g.Key == 7 ? "Jul" : g.Key == 8 ? "Ago" : g.Key == 9 ? "Sep" : g.Key == 10 ? "Oct" : g.Key == 11 ? "Nov" : "Dic",
                    Cantidad = g.Count()
                })
                .ToListAsync();

            return Ok(viajes);
        }

        [HttpGet("combustible-por-mes")]
        public async Task<IActionResult> GetCombustiblePorMes([FromQuery] int anio)
        {
            var consumo = await _context.ConsumosCombustible
                .Where(c => c.Fecha.Year == anio)
                .GroupBy(c => c.Fecha.Month)
                .Select(g => new
                {
                    Mes = g.Key == 1 ? "Ene" : g.Key == 2 ? "Feb" : g.Key == 3 ? "Mar" : g.Key == 4 ? "Abr" : g.Key == 5 ? "May" : g.Key == 6 ? "Jun" : g.Key == 7 ? "Jul" : g.Key == 8 ? "Ago" : g.Key == 9 ? "Sep" : g.Key == 10 ? "Oct" : g.Key == 11 ? "Nov" : "Dic",
                    Galones = g.Sum(c => c.Galones)
                })
                .ToListAsync();

            return Ok(consumo);
        }

        [HttpGet("vehiculos-mas-usados")]
        public async Task<IActionResult> GetVehiculosMasUsados()
        {
            var vehiculos = await (from viaje in _context.Viajes
                                   join asignacion in _context.Asignaciones on viaje.AsignacionId equals asignacion.Id
                                   join vehiculo in _context.Vehiculos on asignacion.VehiculoId equals vehiculo.Id
                                   group viaje by new { vehiculo.Marca, vehiculo.Modelo, vehiculo.Matricula } into g
                                   select new
                                   {
                                       Vehiculo = g.Key.Marca + " " + g.Key.Modelo + " (" + g.Key.Matricula + ")",
                                       Viajes = g.Count()
                                   })
                                   .OrderByDescending(x => x.Viajes)
                                   .Take(5)
                                   .ToListAsync();

            return Ok(vehiculos);
        }

        [HttpGet("solicitudes-por-area")]
        public async Task<IActionResult> GetSolicitudesPorArea()
        {
            var solicitudes = await _context.SolicitudesTransporte
                .GroupBy(s => s.AreaSolicitante)
                .Select(g => new
                {
                    Area = g.Key ?? "Sin Área",
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .ToListAsync();

            return Ok(solicitudes);
        }

        [HttpGet("conductores-mas-viajes")]
        public async Task<IActionResult> GetConductoresMasViajes()
        {
            var conductores = await (from viaje in _context.Viajes
                                     join asignacion in _context.Asignaciones on viaje.AsignacionId equals asignacion.Id
                                     join conductor in _context.Conductores on asignacion.ConductorId equals conductor.Id
                                     group viaje by new { conductor.Nombre, conductor.Apellido } into g
                                     select new
                                     {
                                         Conductor = g.Key.Nombre + " " + g.Key.Apellido,
                                         Viajes = g.Count()
                                     })
                                     .OrderByDescending(x => x.Viajes)
                                     .Take(5)
                                     .ToListAsync();

            return Ok(conductores);
        }
    }
}