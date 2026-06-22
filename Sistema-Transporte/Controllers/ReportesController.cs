using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaTransporte.Domain.Enums;
using SistemaTransporte.Infrastructure.Data;
using System.Threading.Tasks;

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