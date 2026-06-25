using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SistemaTransporte.Application.DTOs.Viaje;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Application.Services;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;
using Xunit;

namespace SistemaTransporte.Tests
{
    public class ViajeServiceTests
    {
        private readonly Mock<IRepository<Viaje>> _viajeRepoMock;
        private readonly Mock<IRepository<Asignacion>> _asignacionRepoMock;
        private readonly Mock<IRepository<Vehiculo>> _vehiculoRepoMock;
        private readonly Mock<IRepository<Conductor>> _conductorRepoMock;
        private readonly ViajeService _service;

        public ViajeServiceTests()
        {
            _viajeRepoMock = new Mock<IRepository<Viaje>>();
            _asignacionRepoMock = new Mock<IRepository<Asignacion>>();
            _vehiculoRepoMock = new Mock<IRepository<Vehiculo>>();
            _conductorRepoMock = new Mock<IRepository<Conductor>>();

            _service = new ViajeService(
                _viajeRepoMock.Object,
                _asignacionRepoMock.Object,
                _vehiculoRepoMock.Object,
                _conductorRepoMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_AsignacionNoExiste_DebeLanzarException()
        {
            var dto = new CrearViajeDto { AsignacionId = 99, Origen = "A", Destino = "B", CantidadPasajeros = 2 };
            _asignacionRepoMock.Setup(r => r.GetByIdAsync(dto.AsignacionId)).ReturnsAsync((Asignacion)null!);

            var excepcion = await Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateAsync(dto)
            );

            Assert.Equal("La asignación no existe.", excepcion.Message);
            _viajeRepoMock.Verify(r => r.AddAsync(It.IsAny<Viaje>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_AsignacionNoActiva_DebeLanzarException()
        {
            var dto = new CrearViajeDto { AsignacionId = 1, Origen = "A", Destino = "B", CantidadPasajeros = 2 };
            var asignacionInactiva = new Asignacion { Id = 1, Estado = EstadoAsignacion.Finalizada };

            _asignacionRepoMock.Setup(r => r.GetByIdAsync(dto.AsignacionId)).ReturnsAsync(asignacionInactiva);

            var excepcion = await Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateAsync(dto)
            );

            Assert.Equal("La asignación debe estar activa para iniciar un viaje.", excepcion.Message);
        }

        [Fact]
        public async Task CreateAsync_FlujoExitoso_DebeGuardarViajeEnCurso()
        {
            var dto = new CrearViajeDto { AsignacionId = 1, Origen = "Santo Domingo", Destino = "Santiago", CantidadPasajeros = 4 };
            var asignacionActiva = new Asignacion { Id = 1, Estado = EstadoAsignacion.Activa };

            _asignacionRepoMock.Setup(r => r.GetByIdAsync(dto.AsignacionId)).ReturnsAsync(asignacionActiva);

            var resultado = await _service.CreateAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal(EstadoViaje.EnCurso, resultado.Estado);
            Assert.Equal("Santo Domingo", resultado.Origen);
            _viajeRepoMock.Verify(r => r.AddAsync(It.IsAny<Viaje>()), Times.Once);
            _viajeRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_EstadoFinalizado_DebeLiberarVehiculoYConductor()
        {
            int viajeId = 1;
            var viaje = new Viaje { Id = viajeId, AsignacionId = 10, Estado = EstadoViaje.EnCurso };
            var asignacion = new Asignacion { Id = 10, VehiculoId = 5, ConductorId = 8, Estado = EstadoAsignacion.Activa };
            var vehiculo = new Vehiculo { Id = 5, Estado = EstadoVehiculo.EnViaje };
            var conductor = new Conductor { Id = 8, Estado = EstadoConductor.EnViaje };

            var dto = new ActualizarViajeDto { Estado = EstadoViaje.Finalizado, DistanciaRecorrida = 150, Observaciones = "Sin novedades" };

            _viajeRepoMock.Setup(r => r.GetByIdAsync(viajeId)).ReturnsAsync(viaje);
            _asignacionRepoMock.Setup(r => r.GetByIdAsync(viaje.AsignacionId)).ReturnsAsync(asignacion);
            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(asignacion.VehiculoId)).ReturnsAsync(vehiculo);
            _conductorRepoMock.Setup(r => r.GetByIdAsync(asignacion.ConductorId)).ReturnsAsync(conductor);

            var resultado = await _service.UpdateAsync(viajeId, dto);

            Assert.True(resultado);
            Assert.Equal(EstadoViaje.Finalizado, viaje.Estado);
            Assert.Equal(EstadoAsignacion.Finalizada, asignacion.Estado);
            Assert.Equal(EstadoVehiculo.Disponible, vehiculo.Estado);
            Assert.Equal(EstadoConductor.Disponible, conductor.Estado);
            Assert.NotNull(viaje.FechaHoraLlegada);

            _vehiculoRepoMock.Verify(r => r.Update(vehiculo), Times.Once);
            _conductorRepoMock.Verify(r => r.Update(conductor), Times.Once);
            _asignacionRepoMock.Verify(r => r.Update(asignacion), Times.Once);
            _viajeRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_SiExiste_DebeRemoverYRetornarTrue()
        {
            int id = 1;
            var viaje = new Viaje { Id = id };

            _viajeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(viaje);

            var resultado = await _service.DeleteAsync(id);

            Assert.True(resultado);
            _viajeRepoMock.Verify(r => r.Delete(viaje), Times.Once);
            _viajeRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}