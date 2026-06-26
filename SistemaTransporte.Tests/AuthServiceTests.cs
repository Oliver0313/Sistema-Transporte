using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using SistemaTransporte.Application.DTOs.Auth;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Application.Services;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;
using Xunit;

namespace SistemaTransporte.Tests
{
    public class AuthServiceTests
    {

        private readonly Mock<IRepository<Usuario>> _usuarioRepoMock;
        private readonly Mock<IRepository<Rol>> _rolRepoMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {

            _usuarioRepoMock = new Mock<IRepository<Usuario>>();
            _rolRepoMock = new Mock<IRepository<Rol>>();
            _configMock = new Mock<IConfiguration>();
            _emailServiceMock = new Mock<IEmailService>();


            _authService = new AuthService(
                _usuarioRepoMock.Object,
                _rolRepoMock.Object,
                _configMock.Object,
                _emailServiceMock.Object
            );
        }

        [Fact]
        public async Task LoginAsync_UsuarioInactivo_DebeLanzarUnauthorizedAccessException()
        {

            var dtoLogin = new LoginDto { Correo = "inactivo@transporte.com", Contrasena = "123456" };


            string contrasenaPlana = "123456";
            string hashValido = BCrypt.Net.BCrypt.HashPassword(contrasenaPlana);


            var listaUsuariosFicticia = new List<Usuario>
            {
                new Usuario
                {
                    Id = 1,
                    Correo = "inactivo@transporte.com",
                    ContrasenaHash = hashValido,
                    EstadoUsuario = EstadoUsuario.Inactivo, 
                    RolId = 2
                }
            };

           
            _usuarioRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(listaUsuariosFicticia);

          
            var excepcion = await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _authService.LoginAsync(dtoLogin)
            );

          
            Assert.Equal("Su cuenta está inactiva. Contacte al administrador.", excepcion.Message);
        }

        [Fact]
        public async Task LoginAsync_UsuarioActivoYCredencialesCorrectas_DebeRetornarAuthResponseDto()
        {
           
            var dtoLogin = new LoginDto { Correo = "activo@transporte.com", Contrasena = "123456" };
            string hashValido = BCrypt.Net.BCrypt.HashPassword("123456");

            var listaUsuariosFicticia = new List<Usuario>
            {
                new Usuario
                {
                    Id = 5,
                    Nombre = "Carlos",
                    Correo = "activo@transporte.com",
                    ContrasenaHash = hashValido,
                    EstadoUsuario = EstadoUsuario.Activo,
                    RolId = 1
                }
            };

            var rolFicticio = new Rol { Id = 1, Nombre = "Administrador" };

           
            _usuarioRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(listaUsuariosFicticia);
            _rolRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(rolFicticio);

        
            _configMock.Setup(c => c["Jwt:Key"]).Returns("EstaEsUnaClaveSugeridaSujetaALongitudDeBitsValidaDe256BitsMínimo");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("SistemaTransporteBackend");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("SistemaTransporteFrontend");
            _configMock.Setup(c => c["Jwt:ExpirationMinutes"]).Returns("60");

            
            var resultado = await _authService.LoginAsync(dtoLogin);

           
            Assert.NotNull(resultado);
            Assert.Equal("Carlos", resultado.Nombre);
            Assert.Equal("Administrador", resultado.Rol);
            Assert.NotEmpty(resultado.Token);
        }
    }
}