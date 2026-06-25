using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SistemaTransporte.Application.DTOs.Asignacion;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Application.Services;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;
using Xunit;

namespace SistemaTransporte.Tests
{
    public class AsignacionServiceTests
    {
        private readonly Mock<IRepository<Asignacion>> _asignacionRepoMock;
        private readonly Mock<IRepository<SolicitudTransporte>> _solicitudRepoMock;
        private readonly Mock<IRepository<Vehiculo>> _vehiculoRepoMock;
        private readonly Mock<IRepository<Conductor>> _conductorRepoMock;
        private readonly AsignacionService _service;

        public AsignacionServiceTests()
        {
            _asignacionRepoMock = new Mock<IRepository<Asignacion>>();
            _solicitudRepoMock = new Mock<IRepository<SolicitudTransporte>>();
            _vehiculoRepoMock = new Mock<IRepository<Vehiculo>>();
            _conductorRepoMock = new Mock<IRepository<Conductor>>();

            _service = new AsignacionService(
                _asignacionRepoMock.Object,
                _solicitudRepoMock.Object,
                _vehiculoRepoMock.Object,
                _conductorRepoMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_SolicitudNoAprobada_DebeLanzarException()
        {
            var dto = new CrearAsignacionDto { SolicitudTransporteId = 1, VehiculoId = 1, ConductorId = 1 };
            var solicitudPendiente = new SolicitudTransporte { Id = 1, Estado = EstadoSolicitud.Pendiente };

            _solicitudRepoMock.Setup(r => r.GetByIdAsync(dto.SolicitudTransporteId)).ReturnsAsync(solicitudPendiente);

            var excepcion = await Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateAsync(dto)
            );

            Assert.Equal("La solicitud debe estar aprobada para poder asignarla.", excepcion.Message);
            _asignacionRepoMock.Verify(r => r.AddAsync(It.IsAny<Asignacion>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_VehiculoNoDisponible_DebeLanzarException()
        {
            var dto = new CrearAsignacionDto { SolicitudTransporteId = 1, VehiculoId = 2, ConductorId = 1 };
            var solicitudAprobada = new SolicitudTransporte { Id = 1, Estado = EstadoSolicitud.Aprobada };
            var vehiculoMantenimiento = new Vehiculo { Id = 2, Estado = EstadoVehiculo.EnMantenimiento };

            _solicitudRepoMock.Setup(r => r.GetByIdAsync(dto.SolicitudTransporteId)).ReturnsAsync(solicitudAprobada);
            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(dto.VehiculoId)).ReturnsAsync(vehiculoMantenimiento);

            var excepcion = await Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateAsync(dto)
            );

            Assert.Equal("El vehículo no está disponible.", excepcion.Message);
        }

        [Fact]
        public async Task CreateAsync_ConductorNoDisponible_DebeLanzarException()
        {
            var dto = new CrearAsignacionDto { SolicitudTransporteId = 1, VehiculoId = 1, ConductorId = 3 };
            var solicitudAprobada = new SolicitudTransporte { Id = 1, Estado = EstadoSolicitud.Aprobada };
            var vehiculoDisponible = new Vehiculo { Id = 1, Estado = EstadoVehiculo.Disponible };
            var conductorEnViaje = new Conductor { Id = 3, Estado = EstadoConductor.EnViaje };

            // CORREGIDO AQUÍ: cambiado 'solicitorAprobada' por 'solicitudAprobada'
            _solicitudRepoMock.Setup(r => r.GetByIdAsync(dto.SolicitudTransporteId)).ReturnsAsync(solicitudAprobada);
            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(dto.VehiculoId)).ReturnsAsync(vehiculoDisponible);
            _conductorRepoMock.Setup(r => r.GetByIdAsync(dto.ConductorId)).ReturnsAsync(conductorEnViaje);

            var excepcion = await Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateAsync(dto)
            );

            Assert.Equal("El conductor no está disponible.", excepcion.Message);
        }

        [Fact]
        public async Task CreateAsync_FlujoExitoso_DebeGuardarYCambiarEstadosAEnViaje()
        {
            var dto = new CrearAsignacionDto { SolicitudTransporteId = 1, VehiculoId = 1, ConductorId = 1, UsuarioAsignadorId = 10 };
            var solicitudAprobada = new SolicitudTransporte { Id = 1, Estado = EstadoSolicitud.Aprobada };
            var vehiculo = new Vehiculo { Id = 1, Estado = EstadoVehiculo.Disponible };
            var conductor = new Conductor { Id = 1, Estado = EstadoConductor.Disponible };

            _solicitudRepoMock.Setup(r => r.GetByIdAsync(dto.SolicitudTransporteId)).ReturnsAsync(solicitudAprobada);
            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(dto.VehiculoId)).ReturnsAsync(vehiculo);
            _conductorRepoMock.Setup(r => r.GetByIdAsync(dto.ConductorId)).ReturnsAsync(conductor);

            var resultado = await _service.CreateAsync(dto);

            Assert.NotNull(resultado);
            Assert.Equal(EstadoAsignacion.Activa, resultado.Estado);
            Assert.Equal(EstadoVehiculo.EnViaje, vehiculo.Estado);
            Assert.Equal(EstadoConductor.EnViaje, conductor.Estado);

            _asignacionRepoMock.Verify(r => r.AddAsync(It.IsAny<Asignacion>()), Times.Once);
            _vehiculoRepoMock.Verify(r => r.Update(vehiculo), Times.Once);
            _conductorRepoMock.Verify(r => r.Update(conductor), Times.Once);
            _asignacionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_SiExiste_DebeLiberarRecursosYRetornarTrue()
        {
            int asignacionId = 1;
            var asignacion = new Asignacion { Id = asignacionId, VehiculoId = 5, ConductorId = 8 };
            var vehiculo = new Vehiculo { Id = 5, Estado = EstadoVehiculo.EnViaje };
            var conductor = new Conductor { Id = 8, Estado = EstadoConductor.EnViaje };

            _asignacionRepoMock.Setup(r => r.GetByIdAsync(asignacionId)).ReturnsAsync(asignacion);
            _vehiculoRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(vehiculo);
            _conductorRepoMock.Setup(r => r.GetByIdAsync(8)).ReturnsAsync(conductor);

            var resultado = await _service.DeleteAsync(asignacionId);

            Assert.True(resultado);
            Assert.Equal(EstadoVehiculo.Disponible, vehiculo.Estado);
            Assert.Equal(EstadoConductor.Disponible, conductor.Estado);

            _asignacionRepoMock.Verify(r => r.Delete(asignacion), Times.Once);
            _asignacionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}