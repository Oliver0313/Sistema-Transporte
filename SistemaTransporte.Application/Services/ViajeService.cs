using SistemaTransporte.Application.DTOs.Viaje;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.Services
{
    public class ViajeService : IViajeService
    {
        private readonly IRepository<Viaje> _viajeRepository;
        private readonly IRepository<Asignacion> _asignacionRepository;
        private readonly IRepository<Vehiculo> _vehiculoRepository;
        private readonly IRepository<Conductor> _conductorRepository;

        public ViajeService(
            IRepository<Viaje> viajeRepository,
            IRepository<Asignacion> asignacionRepository,
            IRepository<Vehiculo> vehiculoRepository,
            IRepository<Conductor> conductorRepository)
        {
            _viajeRepository = viajeRepository;
            _asignacionRepository = asignacionRepository;
            _vehiculoRepository = vehiculoRepository;
            _conductorRepository = conductorRepository;
        }

        public async Task<IEnumerable<ViajeDto>> GetAllAsync()
        {
            var viajes = await _viajeRepository.GetAllAsync();

            return viajes.Select(v => new ViajeDto
            {
                Id = v.Id,
                AsignacionId = v.AsignacionId,
                FechaHoraSalida = v.FechaHoraSalida,
                FechaHoraLlegada = v.FechaHoraLlegada,
                Estado = v.Estado,
                Observaciones = v.Observaciones,
                Origen = v.Origen,
                Destino = v.Destino,
                CantidadPasajeros = v.CantidadPasajeros,
                DistanciaRecorrida = v.DistanciaRecorrida
            });
        }

        public async Task<ViajeDto?> GetByIdAsync(int id)
        {
            var v = await _viajeRepository.GetByIdAsync(id);

            if (v == null)
                return null;

            return new ViajeDto
            {
                Id = v.Id,
                AsignacionId = v.AsignacionId,
                FechaHoraSalida = v.FechaHoraSalida,
                FechaHoraLlegada = v.FechaHoraLlegada,
                Estado = v.Estado,
                Observaciones = v.Observaciones,
                Origen = v.Origen,
                Destino = v.Destino,
                CantidadPasajeros = v.CantidadPasajeros,
                DistanciaRecorrida = v.DistanciaRecorrida
            };
        }

        public async Task<ViajeDto> CreateAsync(CrearViajeDto dto)
        {
            var asignacion = await _asignacionRepository.GetByIdAsync(dto.AsignacionId);

            if (asignacion == null)
                throw new Exception("La asignación no existe.");

            if (asignacion.Estado != EstadoAsignacion.Activa)
                throw new Exception("La asignación debe estar activa para iniciar un viaje.");

            var viaje = new Viaje
            {
                AsignacionId = dto.AsignacionId,
                FechaHoraSalida = DateTime.Now,
                Estado = EstadoViaje.EnCurso,
                Origen = dto.Origen,
                Destino = dto.Destino,
                CantidadPasajeros = dto.CantidadPasajeros,
                Observaciones = string.Empty,
                DistanciaRecorrida = 0
            };

            await _viajeRepository.AddAsync(viaje);
            await _viajeRepository.SaveChangesAsync();

            return new ViajeDto
            {
                Id = viaje.Id,
                AsignacionId = viaje.AsignacionId,
                FechaHoraSalida = viaje.FechaHoraSalida,
                FechaHoraLlegada = viaje.FechaHoraLlegada,
                Estado = viaje.Estado,
                Observaciones = viaje.Observaciones,
                Origen = viaje.Origen,
                Destino = viaje.Destino,
                CantidadPasajeros = viaje.CantidadPasajeros,
                DistanciaRecorrida = viaje.DistanciaRecorrida
            };
        }

        public async Task<bool> UpdateAsync(int id, ActualizarViajeDto dto)
        {
            var viaje = await _viajeRepository.GetByIdAsync(id);

            if (viaje == null)
                return false;

            viaje.Estado = dto.Estado;
            viaje.Observaciones = dto.Observaciones;
            viaje.DistanciaRecorrida = dto.DistanciaRecorrida;
            viaje.FechaHoraLlegada = dto.FechaHoraLlegada;

            if (dto.Estado == EstadoViaje.Finalizado)
            {
                viaje.FechaHoraLlegada ??= DateTime.Now;

                var asignacion = await _asignacionRepository.GetByIdAsync(viaje.AsignacionId);

                if (asignacion != null)
                {
                    asignacion.Estado = EstadoAsignacion.Finalizada;

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

                    _asignacionRepository.Update(asignacion);
                }
            }

            _viajeRepository.Update(viaje);
            await _viajeRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var viaje = await _viajeRepository.GetByIdAsync(id);

            if (viaje == null)
                return false;

            _viajeRepository.Delete(viaje);
            await _viajeRepository.SaveChangesAsync();

            return true;
        }
    }
}