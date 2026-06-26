using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using SistemaTransporte.Application.DTOs.SolicitudTransporte;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Application.Services;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;
using Xunit;

namespace SistemaTransporte.Tests
{
    public class SolicitudTransporteServiceTests
    {
        private readonly Mock<IRepository<SolicitudTransporte>> _solicitudRepoMock;
        private readonly Mock<IRepository<Asignacion>> _asignacionRepoMock;
        private readonly Mock<IRepository<Vehiculo>> _vehiculoRepoMock;
        private readonly Mock<IRepository<Conductor>> _conductorRepoMock;
        private readonly SolicitudTransporteService _service;

        public SolicitudTransporteServiceTests()
        {
            _solicitudRepoMock = new Mock<IRepository<SolicitudTransporte>>();
            _asignacionRepoMock = new Mock<IRepository<Asignacion>>();
            _vehiculoRepoMock = new Mock<IRepository<Vehiculo>>();
            _conductorRepoMock = new Mock<IRepository<Conductor>>();

            _service = new SolicitudTransporteService(
                _solicitudRepoMock.Object,
                _asignacionRepoMock.Object,
                _vehiculoRepoMock.Object,
                _conductorRepoMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_FechaRegresoMenorOSigualALaSalida_DebeLanzarException()
        {
         
            var dto = new CrearSolicitudTransporteDto
            {
                AreaSolicitante = "Tecnología",
                CantidadColaboradores = 3,
                FechaHoraSalida = DateTime.Now.AddHours(2),
                FechaHoraRegreso = DateTime.Now.AddHours(1), 
                Destino = "Santiago",
                Motivo = "Mantenimiento de Servidores"
            };

         
            var excepcion = await Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateAsync(dto)
            );

            Assert.Equal("La hora de regreso debe ser mayor que la hora de salida.", excepcion.Message);
            _solicitudRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never); 
        }

        [Fact]
        public async Task CreateAsync_CantidadColaboradoresMenorOEqualACero_DebeLanzarException()
        {
         
            var dto = new CrearSolicitudTransporteDto
            {
                AreaSolicitante = "Operaciones",
                CantidadColaboradores = 0, 
                FechaHoraSalida = DateTime.Now,
                FechaHoraRegreso = DateTime.Now.AddHours(3)
            };

         
            var excepcion = await Assert.ThrowsAsync<Exception>(async () =>
                await _service.CreateAsync(dto)
            );

            Assert.Equal("La cantidad de colaboradores debe ser mayor que cero.", excepcion.Message);
        }

        [Fact]
        public async Task UpdateAsync_ConVehiculoYConductorNuevos_DebeCrearAsignacionCorrectamente()
        {
          
            int solicitudId = 5;
            var solicitudExistente = new SolicitudTransporte { Id = solicitudId, AreaSolicitante = "Ventas" };

          
            var dtoActualizar = new ActualizarSolicitudTransporteDto
            {
                CantidadColaboradores = 2,
                VehiculoId = 12,  
                ConductorId = 8,    
                Estado = EstadoSolicitud.Aprobada,
                UsuarioSolicitanteId = 1
            };

            _solicitudRepoMock.Setup(r => r.GetByIdAsync(solicitudId)).ReturnsAsync(solicitudExistente);

          
            _asignacionRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Asignacion>());

           
            var resultado = await _service.UpdateAsync(solicitudId, dtoActualizar);

           
            Assert.True(resultado);
            Assert.Equal(EstadoSolicitud.Aprobada, solicitudExistente.Estado);

           
            _asignacionRepoMock.Verify(r => r.AddAsync(It.Is<Asignacion>(a =>
                a.SolicitudTransporteId == solicitudId &&
                a.VehiculoId == 12 &&
                a.ConductorId == 8
            )), Times.Once);

            _asignacionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _solicitudRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}