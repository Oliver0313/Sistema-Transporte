using SistemaTransporte.Application.DTOs.Conductor;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;


namespace SistemaTransporte.Application.Services
{
    public class ConductorService : IConductorService
    {
        private readonly IRepository<Conductor> _repository;

        public ConductorService(IRepository<Conductor> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ConductorDto>> GetAllAsync()
        {
            var conductores = await _repository.GetAllAsync();

            return conductores.Select(c => new ConductorDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Cedula = c.Cedula,
                Email = c.Email,
                Licencia = c.Licencia,
                TipoLicencia = c.TipoLicencia,
                FechaVencimientoLicencia = c.FechaVencimientoLicencia,
                Telefono = c.Telefono,
                Direccion = c.Direccion,
                SupervisorId = c.SupervisorId,
                Estado = c.Estado
            });
        }

        public async Task<ConductorDto?> GetByIdAsync(int id)
        {
            var c = await _repository.GetByIdAsync(id);

            if (c == null)
                return null;

            return new ConductorDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Apellido = c.Apellido,
                Cedula = c.Cedula,
                Email = c.Email,
                Licencia = c.Licencia,
                TipoLicencia = c.TipoLicencia,
                FechaVencimientoLicencia = c.FechaVencimientoLicencia,
                Telefono = c.Telefono,
                Direccion = c.Direccion,
                SupervisorId = c.SupervisorId,
                Estado = c.Estado
            };
        }

        public async Task<ConductorDto> CreateAsync(CrearConductorDto dto)
        {
            var conductor = new Conductor
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Cedula = dto.Cedula,
                Email = dto.Email,
                Licencia = dto.Licencia,
                TipoLicencia = dto.TipoLicencia,
                FechaVencimientoLicencia = dto.FechaVencimientoLicencia,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                SupervisorId = dto.SupervisorId,
                Estado = EstadoConductor.Disponible
            };

            await _repository.AddAsync(conductor);
            await _repository.SaveChangesAsync();

            return new ConductorDto
            {
                Id = conductor.Id,
                Nombre = conductor.Nombre,
                Apellido = conductor.Apellido,
                Cedula = conductor.Cedula,
                Email = conductor.Email,
                Licencia = conductor.Licencia,
                TipoLicencia = conductor.TipoLicencia,
                FechaVencimientoLicencia = conductor.FechaVencimientoLicencia,
                Telefono = conductor.Telefono,
                Direccion = conductor.Direccion,
                SupervisorId = conductor.SupervisorId,
                Estado = conductor.Estado
            };
        }

        public async Task<bool> UpdateAsync(int id, ActualizarConductorDto dto)
        {
            var conductor = await _repository.GetByIdAsync(id);

            if (conductor == null)
                return false;

            conductor.Nombre = dto.Nombre;
            conductor.Apellido = dto.Apellido;
            conductor.Cedula = dto.Cedula;
            conductor.Email = dto.Email;
            conductor.Licencia = dto.Licencia;
            conductor.TipoLicencia = dto.TipoLicencia;
            conductor.FechaVencimientoLicencia = dto.FechaVencimientoLicencia;
            conductor.Telefono = dto.Telefono;
            conductor.Direccion = dto.Direccion;
            conductor.SupervisorId = dto.SupervisorId;
            conductor.Estado = dto.Estado;

            _repository.Update(conductor);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var conductor = await _repository.GetByIdAsync(id);

            if (conductor == null)
                return false;

            _repository.Delete(conductor);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
