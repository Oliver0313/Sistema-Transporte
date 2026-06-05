using SistemaTransporte.Application.DTOs.ConsumoCombustible;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.Services
{
    public class ConsumoCombustibleService : IConsumoCombustibleService
    {
        private readonly IRepository<ConsumoCombustible> _consumoRepository;
        private readonly IRepository<Vehiculo> _vehiculoRepository;
        private readonly IRepository<Conductor> _conductorRepository;

        public ConsumoCombustibleService(
            IRepository<ConsumoCombustible> consumoRepository,
            IRepository<Vehiculo> vehiculoRepository,
            IRepository<Conductor> conductorRepository)
        {
            _consumoRepository = consumoRepository;
            _vehiculoRepository = vehiculoRepository;
            _conductorRepository = conductorRepository;
        }

        public async Task<IEnumerable<ConsumoCombustibleDto>> GetAllAsync()
        {
            var consumos = await _consumoRepository.GetAllAsync();

            return consumos.Select(c => new ConsumoCombustibleDto
            {
                Id = c.Id,
                VehiculoId = c.VehiculoId,
                ConductorId = c.ConductorId,
                Fecha = c.Fecha,
                Galones = c.Galones,
                Costo = c.Costo,
                KilometrosRecorridos = c.KilometrosRecorridos,
                Estado = c.Estado
            });
        }

        public async Task<ConsumoCombustibleDto?> GetByIdAsync(int id)
        {
            var c = await _consumoRepository.GetByIdAsync(id);

            if (c == null)
                return null;

            return new ConsumoCombustibleDto
            {
                Id = c.Id,
                VehiculoId = c.VehiculoId,
                ConductorId = c.ConductorId,
                Fecha = c.Fecha,
                Galones = c.Galones,
                Costo = c.Costo,
                KilometrosRecorridos = c.KilometrosRecorridos,
                Estado = c.Estado
            };
        }

        public async Task<ConsumoCombustibleDto> CreateAsync(CrearConsumoCombustibleDto dto)
        {
            var vehiculo = await _vehiculoRepository.GetByIdAsync(dto.VehiculoId);

            if (vehiculo == null)
                throw new Exception("El vehículo no existe.");

            var conductor = await _conductorRepository.GetByIdAsync(dto.ConductorId);

            if (conductor == null)
                throw new Exception("El conductor no existe.");

            var consumo = new ConsumoCombustible
            {
                VehiculoId = dto.VehiculoId,
                ConductorId = dto.ConductorId,
                Fecha = dto.Fecha,
                Galones = dto.Galones,
                Costo = dto.Costo,
                KilometrosRecorridos = dto.KilometrosRecorridos,
                Estado = EstadoCombustible.Pendiente
            };

            await _consumoRepository.AddAsync(consumo);
            await _consumoRepository.SaveChangesAsync();

            return new ConsumoCombustibleDto
            {
                Id = consumo.Id,
                VehiculoId = consumo.VehiculoId,
                ConductorId = consumo.ConductorId,
                Fecha = consumo.Fecha,
                Galones = consumo.Galones,
                Costo = consumo.Costo,
                KilometrosRecorridos = consumo.KilometrosRecorridos,
                Estado = consumo.Estado
            };
        }

        public async Task<bool> UpdateAsync(int id, ActualizarConsumoCombustibleDto dto)
        {
            var consumo = await _consumoRepository.GetByIdAsync(id);

            if (consumo == null)
                return false;

            consumo.Galones = dto.Galones;
            consumo.Costo = dto.Costo;
            consumo.KilometrosRecorridos = dto.KilometrosRecorridos;
            consumo.Estado = dto.Estado;

            _consumoRepository.Update(consumo);
            await _consumoRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var consumo = await _consumoRepository.GetByIdAsync(id);

            if (consumo == null)
                return false;

            _consumoRepository.Delete(consumo);
            await _consumoRepository.SaveChangesAsync();

            return true;
        }
    }
}