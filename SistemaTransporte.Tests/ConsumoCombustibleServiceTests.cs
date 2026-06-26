using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SistemaTransporte.Application.DTOs.ConsumoCombustible;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Application.Services;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;
using Xunit;

namespace SistemaTransporte.Tests
{
    public class ConsumoCombustibleServiceTests
    {
        private readonly Mock<IRepository<ConsumoCombustible>> _consumoRepoMock;
        private readonly Mock<IRepository<Vehiculo>> _vehiculoRepoMock;
        private readonly Mock<IRepository<Conductor>> _conductorRepoMock;
        private readonly ConsumoCombustibleService _service;

        public ConsumoCombustibleServiceTests()
        {
            _consumoRepoMock = new Mock<IRepository<ConsumoCombustible>>();
            _vehiculoRepoMock = new Mock<IRepository<Vehiculo>>();
            _conductorRepoMock = new Mock<IRepository<Conductor>>();

            _service = new ConsumoCombustibleService(
                _consumoRepoMock.Object,
                _vehiculoRepoMock.Object,
                _conductorRepoMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_VehiculoNoExiste_DebeLanzarException()
        {
            var dto = new CrearConsumoCombustibleDto { VehiculoId = 99, ConductorId = 1 };
            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(dto.VehiculoId)).ReturnsAsync((Vehiculo)null!);

            var excepcion = await Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateAsync(dto)
            );

            Assert.Equal("El vehículo no existe.", excepcion.Message);
            _consumoRepoMock.Verify(r => r.AddAsync(It.IsAny<ConsumoCombustible>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ConductorNoExiste_DebeLanzarException()
        {
            var dto = new CrearConsumoCombustibleDto { VehiculoId = 1, ConductorId = 99 };
            var vehiculo = new Vehiculo { Id = 1 };

            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(dto.VehiculoId)).ReturnsAsync(vehiculo);
            _conductorRepoMock.Setup(r => r.GetByIdAsync(dto.ConductorId)).ReturnsAsync((Conductor)null!);

            var excepcion = await Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateAsync(dto)
            );

            Assert.Equal("El conductor no existe.", excepcion.Message);
        }

        [Fact]
        public async Task CreateAsync_FlujoExitoso_DebeGuardarConEstadoPendiente()
        {
            var dto = new CrearConsumoCombustibleDto
            {
                VehiculoId = 1,
                ConductorId = 1,
                Fecha = DateTime.Now,
                Galones = 10,
                Costo = 2500,
                KilometrosRecorridos = 120
            };

            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(dto.VehiculoId)).ReturnsAsync(new Vehiculo { Id = 1 });
            _conductorRepoMock.Setup(r => r.GetByIdAsync(dto.ConductorId)).ReturnsAsync(new Conductor { Id = 1 });

            var resultado = await _service.CreateAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal(EstadoCombustible.Pendiente, resultado.Estado);
            _consumoRepoMock.Verify(r => r.AddAsync(It.IsAny<ConsumoCombustible>()), Times.Once);
            _consumoRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_SiExiste_DebeActualizarYRetornarTrue()
        {
            int id = 1;
            var consumoExistente = new ConsumoCombustible { Id = id, Galones = 5, Estado = EstadoCombustible.Pendiente };
            var dto = new ActualizarConsumoCombustibleDto { Galones = 15, Costo = 3000, Estado = EstadoCombustible.Aprobado };

            _consumoRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(consumoExistente);

            var resultado = await _service.UpdateAsync(id, dto);

            Assert.True(resultado);
            Assert.Equal(15, consumoExistente.Galones);
            Assert.Equal(EstadoCombustible.Aprobado, consumoExistente.Estado);
            _consumoRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_SiExiste_DebeRemoverYRetornarTrue()
        {
            int id = 1;
            var consumo = new ConsumoCombustible { Id = id };

            _consumoRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(consumo);

            var resultado = await _service.DeleteAsync(id);

            Assert.True(resultado);
            _consumoRepoMock.Verify(r => r.Delete(consumo), Times.Once);
            _consumoRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}