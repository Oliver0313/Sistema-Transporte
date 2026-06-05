using SistemaTransporte.Application.DTOs.Vehiculo;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.Services
{
    public class VehiculoService : IVehiculoService
    {
        private readonly IRepository<Vehiculo> _repository;

        public VehiculoService(IRepository<Vehiculo> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VehiculoDto>> GetAllAsync()
        {
            var vehiculos = await _repository.GetAllAsync();

            return vehiculos.Select(v => new VehiculoDto
            {
                Id = v.Id,
                Matricula = v.Matricula,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Anio = v.Anio,
                Color = v.Color,
                Capacidad = v.Capacidad,
                Estado = v.Estado,
                FechaUltimoMantenimiento = v.FechaUltimoMantenimiento,
                Kilometraje = v.Kilometraje,
                Tipo = v.Tipo,
                ImagenUrl = v.ImagenUrl
            });
        }

        public async Task<VehiculoDto?> GetByIdAsync(int id)
        {
            var v = await _repository.GetByIdAsync(id);

            if (v == null)
                return null;

            return new VehiculoDto
            {
                Id = v.Id,
                Matricula = v.Matricula,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Anio = v.Anio,
                Color = v.Color,
                Capacidad = v.Capacidad,
                Estado = v.Estado,
                FechaUltimoMantenimiento = v.FechaUltimoMantenimiento,
                Kilometraje = v.Kilometraje,
                Tipo = v.Tipo,
                ImagenUrl = v.ImagenUrl
            };
        }

        public async Task<VehiculoDto> CreateAsync(CrearVehiculoDto dto)
        {
            var vehiculo = new Vehiculo
            {
                Matricula = dto.Matricula,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Anio = dto.Anio,
                Color = dto.Color,
                Capacidad = dto.Capacidad,
                Kilometraje = dto.Kilometraje,
                Tipo = dto.Tipo,
                ImagenUrl = dto.ImagenUrl,
                Estado = EstadoVehiculo.Disponible
            };

            await _repository.AddAsync(vehiculo);
            await _repository.SaveChangesAsync();

            return new VehiculoDto
            {
                Id = vehiculo.Id,
                Matricula = vehiculo.Matricula,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                Anio = vehiculo.Anio,
                Color = vehiculo.Color,
                Capacidad = vehiculo.Capacidad,
                Estado = vehiculo.Estado,
                FechaUltimoMantenimiento = vehiculo.FechaUltimoMantenimiento,
                Kilometraje = vehiculo.Kilometraje,
                Tipo = vehiculo.Tipo,
                ImagenUrl = vehiculo.ImagenUrl
            };
        }

        public async Task<bool> UpdateAsync(int id, ActualizarVehiculoDto dto)
        {
            var vehiculo = await _repository.GetByIdAsync(id);

            if (vehiculo == null)
                return false;

            vehiculo.Matricula = dto.Matricula;
            vehiculo.Marca = dto.Marca;
            vehiculo.Modelo = dto.Modelo;
            vehiculo.Anio = dto.Anio;
            vehiculo.Color = dto.Color;
            vehiculo.Capacidad = dto.Capacidad;
            vehiculo.Kilometraje = dto.Kilometraje;
            vehiculo.Tipo = dto.Tipo;
            vehiculo.ImagenUrl = dto.ImagenUrl;
            vehiculo.Estado = dto.Estado;

            _repository.Update(vehiculo);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vehiculo = await _repository.GetByIdAsync(id);

            if (vehiculo == null)
                return false;

            _repository.Delete(vehiculo);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}