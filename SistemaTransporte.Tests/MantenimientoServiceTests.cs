using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SistemaTransporte.Application.DTOs.Mantenimiento;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Application.Services;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;
using Xunit;

namespace SistemaTransporte.Tests
{
    public class MantenimientoServiceTests
    {
        private readonly Mock<IRepository<Mantenimiento>> _mantenimientoRepoMock;
        private readonly Mock<IRepository<Vehiculo>> _vehiculoRepoMock;
        private readonly MantenimientoService _service;

        public MantenimientoServiceTests()
        {
            _mantenimientoRepoMock = new Mock<IRepository<Mantenimiento>>();
            _vehiculoRepoMock = new Mock<IRepository<Vehiculo>>();

            _service = new MantenimientoService(
                _mantenimientoRepoMock.Object,
                _vehiculoRepoMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_VehiculoNoExiste_DebeLanzarException()
        {
            var dto = new CrearMantenimientoDto { VehiculoId = 99, Descripcion = "Cambio de aceite" };
            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(dto.VehiculoId)).ReturnsAsync((Vehiculo)null!);

            var excepcion = await Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateAsync(dto)
            );

            Assert.Equal("El vehículo no existe.", excepcion.Message);
            _mantenimientoRepoMock.Verify(r => r.AddAsync(It.IsAny<Mantenimiento>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_FlujoExitoso_DebeGuardarComoProgramado()
        {
            var dto = new CrearMantenimientoDto
            {
                VehiculoId = 1,
                FechaMantenimiento = DateTime.Now,
                TipoMantenimiento = TipoMantenimiento.Correctivo, // CORREGIDO AQUÍ
                Descripcion = "Revisión de frenos",
                Costo = 5000,
                Taller = "Taller Central"
            };

            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(dto.VehiculoId)).ReturnsAsync(new Vehiculo { Id = 1 });

            var resultado = await _service.CreateAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal(EstadoMantenimiento.Programado, resultado.Estado);
            _mantenimientoRepoMock.Verify(r => r.AddAsync(It.IsAny<Mantenimiento>()), Times.Once);
            _mantenimientoRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_EstadoEnProgreso_DebeCambiarVehiculoAEnMantenimiento()
        {
            int mantenimientoId = 1;
            var mantenimiento = new Mantenimiento { Id = mantenimientoId, VehiculoId = 10, Estado = EstadoMantenimiento.Programado };
            var vehiculo = new Vehiculo { Id = 10, Estado = EstadoVehiculo.Disponible };
            var dto = new ActualizarMantenimientoDto { Estado = EstadoMantenimiento.EnProgreso, TipoMantenimiento = TipoMantenimiento.Correctivo }; // CORREGIDO AQUÍ

            _mantenimientoRepoMock.Setup(r => r.GetByIdAsync(mantenimientoId)).ReturnsAsync(mantenimiento);
            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(mantenimiento.VehiculoId)).ReturnsAsync(vehiculo);

            var resultado = await _service.UpdateAsync(mantenimientoId, dto);

            Assert.True(resultado);
            Assert.Equal(EstadoMantenimiento.EnProgreso, mantenimiento.Estado);
            Assert.Equal(EstadoVehiculo.EnMantenimiento, vehiculo.Estado);
            _vehiculoRepoMock.Verify(r => r.Update(vehiculo), Times.Once);
            _mantenimientoRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_EstadoCompletado_DebeCambiarVehiculoADisponibleYActualizarFecha()
        {
            int mantenimientoId = 1;
            var mantenimiento = new Mantenimiento { Id = mantenimientoId, VehiculoId = 10, Estado = EstadoMantenimiento.EnProgreso };
            var vehiculo = new Vehiculo { Id = 10, Estado = EstadoVehiculo.EnMantenimiento, FechaUltimoMantenimiento = DateTime.MinValue };
            var dto = new ActualizarMantenimientoDto { Estado = EstadoMantenimiento.Completado, TipoMantenimiento = TipoMantenimiento.Preventivo }; // CORREGIDO AQUÍ

            _mantenimientoRepoMock.Setup(r => r.GetByIdAsync(mantenimientoId)).ReturnsAsync(mantenimiento);
            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(mantenimiento.VehiculoId)).ReturnsAsync(vehiculo);

            var resultado = await _service.UpdateAsync(mantenimientoId, dto);

            Assert.True(resultado);
            Assert.Equal(EstadoMantenimiento.Completado, mantenimiento.Estado);
            Assert.Equal(EstadoVehiculo.Disponible, vehiculo.Estado);
            Assert.True(vehiculo.FechaUltimoMantenimiento > DateTime.MinValue);
            _vehiculoRepoMock.Verify(r => r.Update(vehiculo), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_SiExiste_DebeRemoverYRetornarTrue()
        {
            int id = 1;
            var mantenimiento = new Mantenimiento { Id = id };

            _mantenimientoRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(mantenimiento);

            var resultado = await _service.DeleteAsync(id);

            Assert.True(resultado);
            _mantenimientoRepoMock.Verify(r => r.Delete(mantenimiento), Times.Once);
            _mantenimientoRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}