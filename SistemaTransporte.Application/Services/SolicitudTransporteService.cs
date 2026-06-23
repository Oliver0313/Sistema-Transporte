using SistemaTransporte.Application.DTOs.SolicitudTransporte;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.Services
{
    public class SolicitudTransporteService : ISolicitudTransporteService
    {
        private readonly IRepository<SolicitudTransporte> _repository;
        private readonly IRepository<Asignacion> _asignacionRepository;
        private readonly IRepository<Vehiculo> _vehiculoRepository;
        private readonly IRepository<Conductor> _conductorRepository;

        public SolicitudTransporteService(
            IRepository<SolicitudTransporte> repository,
            IRepository<Asignacion> asignacionRepository,
            IRepository<Vehiculo> vehiculoRepository,
            IRepository<Conductor> conductorRepository)
        {
            _repository = repository;
            _asignacionRepository = asignacionRepository;
            _vehiculoRepository = vehiculoRepository;
            _conductorRepository = conductorRepository;
        }

        public async Task<IEnumerable<SolicitudTransporteDto>> GetAllAsync()
        {
            var solicitudes = await _repository.GetAllAsync();
            var asignaciones = await _asignacionRepository.GetAllAsync();
            var vehiculos = await _vehiculoRepository.GetAllAsync();
            var conductores = await _conductorRepository.GetAllAsync();

            return solicitudes.Select(s =>
            {
                var asignacion = asignaciones
                    .FirstOrDefault(a => a.SolicitudTransporteId == s.Id);

                var vehiculo = asignacion == null
                    ? null
                    : vehiculos.FirstOrDefault(v => v.Id == asignacion.VehiculoId);

                var conductor = asignacion == null
                    ? null
                    : conductores.FirstOrDefault(c => c.Id == asignacion.ConductorId);

                return new SolicitudTransporteDto
                {
                    Id = s.Id,
                    AreaSolicitante = s.AreaSolicitante,
                    CantidadColaboradores = s.CantidadColaboradores,
                    FechaHoraSalida = s.FechaHoraSalida,
                    FechaHoraRegreso = s.FechaHoraRegreso,
                    Destino = s.Destino,
                    Motivo = s.Motivo,
                    Estado = s.Estado,
                    UsuarioSolicitanteId = s.UsuarioSolicitanteId,
                    VehiculoAsignado = vehiculo == null
                        ? null
                        : $"{vehiculo.Marca} {vehiculo.Modelo} - {vehiculo.Matricula}",
                    ConductorAsignado = conductor == null
                        ? null
                        : $"{conductor.Nombre} {conductor.Apellido}"
                };
            });
        }

        public async Task<SolicitudTransporteDto?> GetByIdAsync(int id)
        {
            var s = await _repository.GetByIdAsync(id);

            if (s == null)
                return null;

            return new SolicitudTransporteDto
            {
                Id = s.Id,
                AreaSolicitante = s.AreaSolicitante,
                CantidadColaboradores = s.CantidadColaboradores,
                FechaHoraSalida = s.FechaHoraSalida,
                FechaHoraRegreso = s.FechaHoraRegreso,
                Destino = s.Destino,
                Motivo = s.Motivo,
                Estado = s.Estado,
                UsuarioSolicitanteId = s.UsuarioSolicitanteId
            };
        }

        public async Task<SolicitudTransporteDto> CreateAsync(CrearSolicitudTransporteDto dto)
        {
            if (dto.CantidadColaboradores <= 0)
                throw new Exception("La cantidad de colaboradores debe ser mayor que cero.");

            if (dto.FechaHoraRegreso <= dto.FechaHoraSalida)
                throw new Exception("La hora de regreso debe ser mayor que la hora de salida.");

            var solicitud = new SolicitudTransporte
            {
                AreaSolicitante = dto.AreaSolicitante,
                CantidadColaboradores = dto.CantidadColaboradores,
                FechaHoraSalida = dto.FechaHoraSalida,
                FechaHoraRegreso = dto.FechaHoraRegreso,
                Destino = dto.Destino,
                Motivo = dto.Motivo,
                UsuarioSolicitanteId = dto.UsuarioSolicitanteId,
                Estado = EstadoSolicitud.Pendiente
            };

            await _repository.AddAsync(solicitud);
            await _repository.SaveChangesAsync();

            return new SolicitudTransporteDto
            {
                Id = solicitud.Id,
                AreaSolicitante = solicitud.AreaSolicitante,
                CantidadColaboradores = solicitud.CantidadColaboradores,
                FechaHoraSalida = solicitud.FechaHoraSalida,
                FechaHoraRegreso = solicitud.FechaHoraRegreso,
                Destino = solicitud.Destino,
                Motivo = solicitud.Motivo,
                Estado = solicitud.Estado,
                UsuarioSolicitanteId = solicitud.UsuarioSolicitanteId
            };
        }

        public async Task<bool> UpdateAsync(int id, ActualizarSolicitudTransporteDto dto)
        {
            var solicitud = await _repository.GetByIdAsync(id);

            if (solicitud == null)
                return false;

            if (dto.CantidadColaboradores <= 0)
                throw new Exception("La cantidad de colaboradores debe ser mayor que cero.");

            if (dto.FechaHoraRegreso <= dto.FechaHoraSalida)
                throw new Exception("La hora de regreso debe ser mayor que la hora de salida.");

            solicitud.AreaSolicitante = dto.AreaSolicitante;
            solicitud.CantidadColaboradores = dto.CantidadColaboradores;
            solicitud.FechaHoraSalida = dto.FechaHoraSalida;
            solicitud.FechaHoraRegreso = dto.FechaHoraRegreso;
            solicitud.Destino = dto.Destino;
            solicitud.Motivo = dto.Motivo;
            solicitud.Estado = dto.Estado;

            _repository.Update(solicitud);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var solicitud = await _repository.GetByIdAsync(id);

            if (solicitud == null)
                return false;

            _repository.Delete(solicitud);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
