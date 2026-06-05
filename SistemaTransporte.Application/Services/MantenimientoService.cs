using SistemaTransporte.Application.DTOs.Mantenimiento;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.Services
{
    public class MantenimientoService : IMantenimientoService
    {
        private readonly IRepository<Mantenimiento> _mantenimientoRepository;
        private readonly IRepository<Vehiculo> _vehiculoRepository;

        public MantenimientoService(
            IRepository<Mantenimiento> mantenimientoRepository,
            IRepository<Vehiculo> vehiculoRepository)
        {
            _mantenimientoRepository = mantenimientoRepository;
            _vehiculoRepository = vehiculoRepository;
        }

        public async Task<IEnumerable<MantenimientoDto>> GetAllAsync()
        {
            var mantenimientos = await _mantenimientoRepository.GetAllAsync();

            return mantenimientos.Select(m => new MantenimientoDto
            {
                Id = m.Id,
                VehiculoId = m.VehiculoId,
                FechaMantenimiento = m.FechaMantenimiento,
                Estado = m.Estado,
                TipoMantenimiento = m.TipoMantenimiento,
                Descripcion = m.Descripcion,
                Costo = m.Costo,
                Taller = m.Taller,
                ProximoMantenimiento = m.ProximoMantenimiento
            });
        }

        public async Task<MantenimientoDto?> GetByIdAsync(int id)
        {
            var m = await _mantenimientoRepository.GetByIdAsync(id);

            if (m == null)
                return null;

            return new MantenimientoDto
            {
                Id = m.Id,
                VehiculoId = m.VehiculoId,
                FechaMantenimiento = m.FechaMantenimiento,
                Estado = m.Estado,
                TipoMantenimiento = m.TipoMantenimiento,
                Descripcion = m.Descripcion,
                Costo = m.Costo,
                Taller = m.Taller,
                ProximoMantenimiento = m.ProximoMantenimiento
            };
        }

        public async Task<MantenimientoDto> CreateAsync(CrearMantenimientoDto dto)
        {
            var vehiculo = await _vehiculoRepository.GetByIdAsync(dto.VehiculoId);

            if (vehiculo == null)
                throw new Exception("El vehículo no existe.");

            var mantenimiento = new Mantenimiento
            {
                VehiculoId = dto.VehiculoId,
                FechaMantenimiento = dto.FechaMantenimiento,
                TipoMantenimiento = dto.TipoMantenimiento,
                Descripcion = dto.Descripcion,
                Costo = dto.Costo,
                Taller = dto.Taller,
                ProximoMantenimiento = dto.ProximoMantenimiento,
                Estado = EstadoMantenimiento.Programado
            };

            await _mantenimientoRepository.AddAsync(mantenimiento);
            await _mantenimientoRepository.SaveChangesAsync();

            return new MantenimientoDto
            {
                Id = mantenimiento.Id,
                VehiculoId = mantenimiento.VehiculoId,
                FechaMantenimiento = mantenimiento.FechaMantenimiento,
                Estado = mantenimiento.Estado,
                TipoMantenimiento = mantenimiento.TipoMantenimiento,
                Descripcion = mantenimiento.Descripcion,
                Costo = mantenimiento.Costo,
                Taller = mantenimiento.Taller,
                ProximoMantenimiento = mantenimiento.ProximoMantenimiento
            };
        }

        public async Task<bool> UpdateAsync(int id, ActualizarMantenimientoDto dto)
        {
            var mantenimiento = await _mantenimientoRepository.GetByIdAsync(id);

            if (mantenimiento == null)
                return false;

            var vehiculo = await _vehiculoRepository.GetByIdAsync(mantenimiento.VehiculoId);

            mantenimiento.Estado = dto.Estado;
            mantenimiento.TipoMantenimiento = dto.TipoMantenimiento;
            mantenimiento.Descripcion = dto.Descripcion;
            mantenimiento.Costo = dto.Costo;
            mantenimiento.Taller = dto.Taller;
            mantenimiento.ProximoMantenimiento = dto.ProximoMantenimiento;

            if (vehiculo != null)
            {
                if (dto.Estado == EstadoMantenimiento.EnProgreso)
                {
                    vehiculo.Estado = EstadoVehiculo.EnMantenimiento;
                    _vehiculoRepository.Update(vehiculo);
                }

                if (dto.Estado == EstadoMantenimiento.Completado)
                {
                    vehiculo.Estado = EstadoVehiculo.Disponible;
                    vehiculo.FechaUltimoMantenimiento = DateTime.Now;
                    _vehiculoRepository.Update(vehiculo);
                }
            }

            _mantenimientoRepository.Update(mantenimiento);
            await _mantenimientoRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var mantenimiento = await _mantenimientoRepository.GetByIdAsync(id);

            if (mantenimiento == null)
                return false;

            _mantenimientoRepository.Delete(mantenimiento);
            await _mantenimientoRepository.SaveChangesAsync();

            return true;
        }
    }
}