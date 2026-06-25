using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SistemaTransporte.Application.DTOs.Vehiculo;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Application.Services;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;
using Xunit;

namespace SistemaTransporte.Tests
{
    public class VehiculoServiceTests
    {
        private readonly Mock<IRepository<Vehiculo>> _repositoryMock;
        private readonly VehiculoService _service;

        public VehiculoServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Vehiculo>>();
            _service = new VehiculoService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_DebeRetornarTodosLosVehiculosMapeados()
        {
            var listaFicticia = new List<Vehiculo>
            {
                new Vehiculo { Id = 1, Marca = "Toyota", Modelo = "Hiace", Estado = EstadoVehiculo.Disponible },
                new Vehiculo { Id = 2, Marca = "Hyundai", Modelo = "H1", Estado = EstadoVehiculo.EnMantenimiento }
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(listaFicticia);

            var resultado = (await _service.GetAllAsync()).ToList();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Toyota", resultado[0].Marca);
            Assert.Equal(EstadoVehiculo.Disponible, resultado[0].Estado);
            Assert.Equal("Hyundai", resultado[1].Marca);
            Assert.Equal(EstadoVehiculo.EnMantenimiento, resultado[1].Estado);
        }

        [Fact]
        public async Task GetByIdAsync_SiExiste_DebeRetornarVehiculoDto()
        {
            int id = 1;
            var vehiculo = new Vehiculo { Id = id, Marca = "Toyota", Matricula = "A123456" };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(vehiculo);

            var resultado = await _service.GetByIdAsync(id);

            Assert.NotNull(resultado);
            Assert.Equal(id, resultado.Id);
            Assert.Equal("Toyota", resultado.Marca);
            Assert.Equal("A123456", resultado.Matricula);
        }

        [Fact]
        public async Task GetByIdAsync_SiNoExiste_DebeRetornarNull()
        {
            int id = 99;
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Vehiculo)null!);

            var resultado = await _service.GetByIdAsync(id);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task CreateAsync_DebeGuardarYRetornarVehiculoDisponible()
        {
            var dto = new CrearVehiculoDto
            {
                Marca = "Toyota",
                Modelo = "Corolla",
                Matricula = "A789012"
            };

            var resultado = await _service.CreateAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Toyota", resultado.Marca);
            Assert.Equal(EstadoVehiculo.Disponible, resultado.Estado);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Vehiculo>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SiExiste_DebeActualizarYRetornarTrue()
        {
            int id = 1;
            var vehiculoExistente = new Vehiculo { Id = id, Modelo = "Antiguo" };
            var dto = new ActualizarVehiculoDto { Modelo = "Nuevo", Estado = EstadoVehiculo.EnViaje };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(vehiculoExistente);

            var resultado = await _service.UpdateAsync(id, dto);

            Assert.True(resultado);
            Assert.Equal("Nuevo", vehiculoExistente.Modelo);
            Assert.Equal(EstadoVehiculo.EnViaje, vehiculoExistente.Estado);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_SiExiste_DebeRemoverYRetornarTrue()
        {
            int id = 1;
            var vehiculo = new Vehiculo { Id = id };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(vehiculo);

            var resultado = await _service.DeleteAsync(id);

            Assert.True(resultado);
            _repositoryMock.Verify(r => r.Delete(vehiculo), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}