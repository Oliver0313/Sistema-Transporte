using SistemaTransporte.Application.DTOs.Asignacion;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.Services
{
    public class AsignacionService : IAsignacionService
    {
        private readonly IRepository<Asignacion> _asignacionRepository;
        private readonly IRepository<SolicitudTransporte> _solicitudRepository;
        private readonly IRepository<Vehiculo> _vehiculoRepository;
        private readonly IRepository<Conductor> _conductorRepository;

        public AsignacionService(
            IRepository<Asignacion> asignacionRepository,
            IRepository<SolicitudTransporte> solicitudRepository,
            IRepository<Vehiculo> vehiculoRepository,
            IRepository<Conductor> conductorRepository)
        {
            _asignacionRepository = asignacionRepository;
            _solicitudRepository = solicitudRepository;
            _vehiculoRepository = vehiculoRepository;
            _conductorRepository = conductorRepository;
        }

        public async Task<AsignacionDto> CreateAsync(CrearAsignacionDto dto)
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(dto.SolicitudTransporteId);

            if (solicitud == null)
                throw new Exception("La solicitud no existe.");

            if (solicitud.Estado != EstadoSolicitud.Aprobada)
                throw new Exception("La solicitud debe estar aprobada para poder asignarla.");

            var vehiculo = await _vehiculoRepository.GetByIdAsync(dto.VehiculoId);

            if (vehiculo == null)
                throw new Exception("El vehículo no existe.");

            if (vehiculo.Estado != EstadoVehiculo.Disponible)
                throw new Exception("El vehículo no está disponible.");

            var conductor = await _conductorRepository.GetByIdAsync(dto.ConductorId);

            if (conductor == null)
                throw new Exception("El conductor no existe.");

            if (conductor.Estado != EstadoConductor.Disponible)
                throw new Exception("El conductor no está disponible.");

            var asignacion = new Asignacion
            {
                SolicitudTransporteId = dto.SolicitudTransporteId,
                VehiculoId = dto.VehiculoId,
                ConductorId = dto.ConductorId,
                UsuarioAsignadorId = dto.UsuarioAsignadorId,
                FechaHoraAsignacion = DateTime.Now,
                Estado = EstadoAsignacion.Activa
            };

            await _asignacionRepository.AddAsync(asignacion);

            vehiculo.Estado = EstadoVehiculo.EnViaje;
            conductor.Estado = EstadoConductor.EnViaje;

            _vehiculoRepository.Update(vehiculo);
            _conductorRepository.Update(conductor);

            await _asignacionRepository.SaveChangesAsync();

            return new AsignacionDto
            {
                Id = asignacion.Id,
                SolicitudTransporteId = asignacion.SolicitudTransporteId,
                VehiculoId = asignacion.VehiculoId,
                ConductorId = asignacion.ConductorId,
                UsuarioAsignadorId = asignacion.UsuarioAsignadorId,
                FechaHoraAsignacion = asignacion.FechaHoraAsignacion,
                Estado = asignacion.Estado
            };
        }

        public async Task<IEnumerable<AsignacionDto>> GetAllAsync()
        {
            var asignaciones = await _asignacionRepository.GetAllAsync();

            return asignaciones.Select(a => new AsignacionDto
            {
                Id = a.Id,
                SolicitudTransporteId = a.SolicitudTransporteId,
                VehiculoId = a.VehiculoId,
                ConductorId = a.ConductorId,
                FechaHoraAsignacion = a.FechaHoraAsignacion,
                UsuarioAsignadorId = a.UsuarioAsignadorId,
                Estado = a.Estado
            });
        }

        public async Task<AsignacionDto?> GetByIdAsync(int id)
        {
            var a = await _asignacionRepository.GetByIdAsync(id);

            if (a == null)
                return null;

            return new AsignacionDto
            {
                Id = a.Id,
                SolicitudTransporteId = a.SolicitudTransporteId,
                VehiculoId = a.VehiculoId,
                ConductorId = a.ConductorId,
                FechaHoraAsignacion = a.FechaHoraAsignacion,
                UsuarioAsignadorId = a.UsuarioAsignadorId,
                Estado = a.Estado
            };
        }

        public async Task<bool> UpdateAsync(int id, ActualizarAsignacionDto dto)
        {
            var asignacion = await _asignacionRepository.GetByIdAsync(id);

            if (asignacion == null)
                return false;

            var nuevoVehiculo = await _vehiculoRepository.GetByIdAsync(dto.VehiculoId);

            if (nuevoVehiculo == null)
                throw new Exception("El vehículo no existe.");

            var nuevoConductor = await _conductorRepository.GetByIdAsync(dto.ConductorId);

            if (nuevoConductor == null)
                throw new Exception("El conductor no existe.");

            asignacion.VehiculoId = dto.VehiculoId;
            asignacion.ConductorId = dto.ConductorId;
            asignacion.Estado = dto.Estado;

            _asignacionRepository.Update(asignacion);
            await _asignacionRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var asignacion = await _asignacionRepository.GetByIdAsync(id);

            if (asignacion == null)
                return false;

            var vehiculo = await _vehiculoRepository.GetByIdAsync(asignacion.VehiculoId);
            var conductor = await _conductorRepository.GetByIdAsync(asignacion.ConductorId);

            if (vehiculo != null)
            {
                vehiculo.Estado = EstadoVehiculo.Disponible;
                _vehiculoRepository.Update(vehiculo);
            }

            if (conductor != null)
            {
                conductor.Estado = EstadoConductor.Disponible;
                _conductorRepository.Update(conductor);
            }

            _asignacionRepository.Delete(asignacion);

            await _asignacionRepository.SaveChangesAsync();

            return true;
        }
    }
}