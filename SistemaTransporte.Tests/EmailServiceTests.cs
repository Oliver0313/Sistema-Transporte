using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using SistemaTransporte.Application.Services;
using Xunit;

namespace SistemaTransporte.Tests
{
    public class EmailServiceTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly EmailService _service;

        public EmailServiceTests()
        {
            _configMock = new Mock<IConfiguration>();
            _service = new EmailService(_configMock.Object);
        }

        [Fact]
        public async Task SendEmailAsync_ConfiguracionInvalida_DebeLanzarFormatException()
        {
            _configMock.Setup(c => c["EmailSettings:Port"]).Returns("NoSoyUnNumero");

            await Assert.ThrowsAsync<FormatException>(async () =>
                await _service.SendEmailAsync("test@correo.com", "Asunto", "Cuerpo")
            );
        }

        [Fact]
        public async Task SendEmailAsync_FaltaPuertoEnConfiguracion_DebeLanzarArgumentNullException()
        {
            _configMock.Setup(c => c["EmailSettings:Port"]).Returns((string)null!);

            // CORREGIDO AQUÍ: Se cambió NullReferenceException por ArgumentNullException
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.SendEmailAsync("test@correo.com", "Asunto", "Cuerpo")
            );
        }
    }
}