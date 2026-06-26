using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SistemaTransporte.Application.DTOs.Usuario;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Application.Services;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;
using Xunit;

namespace SistemaTransporte.Tests
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IRepository<Usuario>> _usuarioRepoMock;
        private readonly Mock<IRepository<Rol>> _rolRepoMock;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _usuarioRepoMock = new Mock<IRepository<Usuario>>();
            _rolRepoMock = new Mock<IRepository<Rol>>();


            _usuarioService = new UsuarioService(_usuarioRepoMock.Object, _rolRepoMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_DebeRetornarUsuariosMapeadosConSuRol()
        {

            var listaRoles = new List<Rol>
            {
                new Rol { Id = 1, Nombre = "Administrador" },
                new Rol { Id = 2, Nombre = "Operador" }
            };

            var listaUsuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nombre = "Juan", RolId = 1, EstadoUsuario = EstadoUsuario.Activo },
                new Usuario { Id = 2, Nombre = "Pedro", RolId = 2, EstadoUsuario = EstadoUsuario.Inactivo }
            };

            _usuarioRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(listaUsuarios);
            _rolRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(listaRoles);

          
            var resultado = (await _usuarioService.GetAllAsync()).ToList();

         
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);


            Assert.Equal("Administrador", resultado[0].RolNombre);
            Assert.Equal(EstadoUsuario.Activo, resultado[0].EstadoUsuario);


            Assert.Equal("Operador", resultado[1].RolNombre);
            Assert.Equal(EstadoUsuario.Inactivo, resultado[1].EstadoUsuario);
        }

        [Fact]
        public async Task UpdateAsync_UsuarioYRolExisten_DebeActualizarYRetornarTrue()
        {

            int usuarioId = 10;
            var usuarioExistente = new Usuario { Id = usuarioId, RolId = 1, EstadoUsuario = EstadoUsuario.Activo };
            var dtoActualizar = new ActualizarUsuarioDto { RolId = 2, EstadoUsuario = EstadoUsuario.Inactivo };
            var rolNuevo = new Rol { Id = 2, Nombre = "Operador" };

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(usuarioId)).ReturnsAsync(usuarioExistente);
            _rolRepoMock.Setup(r => r.GetByIdAsync(dtoActualizar.RolId)).ReturnsAsync(rolNuevo);

            var resultado = await _usuarioService.UpdateAsync(usuarioId, dtoActualizar);

         
            Assert.True(resultado);
            Assert.Equal(EstadoUsuario.Inactivo, usuarioExistente.EstadoUsuario);
            Assert.Equal(2, usuarioExistente.RolId); 

          
            _usuarioRepoMock.Verify(r => r.Update(usuarioExistente), Times.Once);
            _usuarioRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_RolNoExiste_DebeLanzarException()
        {
         
            int usuarioId = 10;
            var usuarioExistente = new Usuario { Id = usuarioId, RolId = 1 };
            var dtoActualizar = new ActualizarUsuarioDto { RolId = 99, EstadoUsuario = EstadoUsuario.Activo }; 

            _usuarioRepoMock.Setup(r => r.GetByIdAsync(usuarioId)).ReturnsAsync(usuarioExistente);
            _rolRepoMock.Setup(r => r.GetByIdAsync(dtoActualizar.RolId)).ReturnsAsync((Rol)null!);

          
            var excepcion = await Assert.ThrowsAsync<Exception>(async () =>
                await _usuarioService.UpdateAsync(usuarioId, dtoActualizar)
            );

            Assert.Equal("El rol no existe.", excepcion.Message);
        
            _usuarioRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}