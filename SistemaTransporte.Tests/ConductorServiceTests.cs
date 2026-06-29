using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SistemaTransporte.Application.DTOs.Conductor;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Application.Services;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;
using Xunit;

namespace SistemaTransporte.Tests
{
    public class ConductorServiceTests
    {
        private readonly Mock<IRepository<Conductor>> _repositoryMock;
        private readonly ConductorService _service;

        public ConductorServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Conductor>>();
            _service = new ConductorService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_DebeRetornarTodosLosConductoresMapeados()
        {
            var listaFicticia = new List<Conductor>
            {
                new Conductor { Id = 1, Nombre = "Pedro", Apellido = "Ramos", Estado = EstadoConductor.Disponible },
                new Conductor { Id = 2, Nombre = "Luis", Apellido = "Gómez", Estado = EstadoConductor.EnViaje }
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(listaFicticia);

            var resultado = (await _service.GetAllAsync()).ToList();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Pedro", resultado[0].Nombre);
            Assert.Equal(EstadoConductor.Disponible, resultado[0].Estado);
            Assert.Equal("Luis", resultado[1].Nombre);
            Assert.Equal(EstadoConductor.EnViaje, resultado[1].Estado);
        }

        [Fact]
        public async Task GetByIdAsync_SiExiste_DebeRetornarConductorDto()
        {
            int id = 1;
            var conductor = new Conductor { Id = id, Nombre = "Pedro", Cedula = "402-0000000-1" };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(conductor);

            var resultado = await _service.GetByIdAsync(id);

            Assert.NotNull(resultado);
            Assert.Equal(id, resultado.Id);
            Assert.Equal("Pedro", resultado.Nombre);
            Assert.Equal("402-0000000-1", resultado.Cedula);
        }

        [Fact]
        public async Task GetByIdAsync_SiNoExiste_DebeRetornarNull()
        {
            int id = 99;
            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Conductor)null!);

            var resultado = await _service.GetByIdAsync(id);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task CreateAsync_DebeGuardarYRetornarConductorDisponible()
        {
            var dto = new CrearConductorDto
            {
                Nombre = "Juan",
                Apellido = "Díaz",
                Cedula = "001-0000000-2"
            };

            var resultado = await _service.CreateAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Juan", resultado.Nombre);
            Assert.Equal(EstadoConductor.Disponible, resultado.Estado);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Conductor>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SiExiste_DebeActualizarYRetornarTrue()
        {
            int id = 1;
            var conductorExistente = new Conductor { Id = id, Nombre = "Anterior" };
            var dto = new ActualizarConductorDto { Nombre = "Nuevo", Estado = EstadoConductor.EnViaje };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(conductorExistente);

            var resultado = await _service.UpdateAsync(id, dto);

            Assert.True(resultado);
            Assert.Equal("Nuevo", conductorExistente.Nombre);
            Assert.Equal(EstadoConductor.EnViaje, conductorExistente.Estado);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_SiExiste_DebeRemoverYRetornarTrue()
        {
            int id = 1;
            var conductor = new Conductor { Id = id };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(conductor);

            var resultado = await _service.DeleteAsync(id);

            Assert.True(resultado);
            _repositoryMock.Verify(r => r.Delete(conductor), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}