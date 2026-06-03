using SistemaTransporte.Application.DTOs.SolicitudTransporte;
using SistemaTransporte.Application.Interfaces;
using SistemaTransporte.Domain.Entities;
using SistemaTransporte.Domain.Enums;

namespace SistemaTransporte.Application.Services
{
    public class SolicitudTransporteService : ISolicitudTransporteService
    {
        private readonly IRepository<SolicitudTransporte> _repository;

        public SolicitudTransporteService(IRepository<SolicitudTransporte> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SolicitudTransporteDto>> GetAllAsync()
        {
            var solicitudes = await _repository.GetAllAsync();

            return solicitudes.Select(s => new SolicitudTransporteDto
            {
                Id = s.Id,
                AreaSolicitante = s.AreaSolicitante,
                CantidadColaboradores = s.CantidadColaboradores,
                FechaHoraSalida = s.FechaHoraSalida,
                FechaHoraRegreso = s.FechaHoraRegreso,
                Destino = s.Destino,
                Motivo = s.Motivo,
                Estado = s.Estado,
                UsuarioSolicitanteId = s.UsuarioSolicitanteId
            });
        }

        public async Task<SolicitudTransporteDto?> GetByIdAsync(int id)
        {
            var s = await _repository.GetByIdAsync(id);

            if (s == null)
                return null;

            return new SolicitudTransporteDto
            {
                Id = s.Id,
                AreaSolicitante = s.AreaSolicitante,
                CantidadColaboradores = s.CantidadColaboradores,
                FechaHoraSalida = s.FechaHoraSalida,
                FechaHoraRegreso = s.FechaHoraRegreso,
                Destino = s.Destino,
                Motivo = s.Motivo,
                Estado = s.Estado,
                UsuarioSolicitanteId = s.UsuarioSolicitanteId
            };
        }

        public async Task<SolicitudTransporteDto> CreateAsync(CrearSolicitudTransporteDto dto)
        {
            if (dto.CantidadColaboradores <= 0)
                throw new Exception("La cantidad de colaboradores debe ser mayor que cero.");

            if (dto.FechaHoraRegreso <= dto.FechaHoraSalida)
                throw new Exception("La hora de regreso debe ser mayor que la hora de salida.");

            var solicitud = new SolicitudTransporte
            {
                AreaSolicitante = dto.AreaSolicitante,
                CantidadColaboradores = dto.CantidadColaboradores,
                FechaHoraSalida = dto.FechaHoraSalida,
                FechaHoraRegreso = dto.FechaHoraRegreso,
                Destino = dto.Destino,
                Motivo = dto.Motivo,
                UsuarioSolicitanteId = dto.UsuarioSolicitanteId,
                Estado = EstadoSolicitud.Pendiente
            };

            await _repository.AddAsync(solicitud);
            await _repository.SaveChangesAsync();

            return new SolicitudTransporteDto
            {
                Id = solicitud.Id,
                AreaSolicitante = solicitud.AreaSolicitante,
                CantidadColaboradores = solicitud.CantidadColaboradores,
                FechaHoraSalida = solicitud.FechaHoraSalida,
                FechaHoraRegreso = solicitud.FechaHoraRegreso,
                Destino = solicitud.Destino,
                Motivo = solicitud.Motivo,
                Estado = solicitud.Estado,
                UsuarioSolicitanteId = solicitud.UsuarioSolicitanteId
            };
        }

        public async Task<bool> UpdateAsync(int id, ActualizarSolicitudTransporteDto dto)
        {
            var solicitud = await _repository.GetByIdAsync(id);

            if (solicitud == null)
                return false;

            if (dto.CantidadColaboradores <= 0)
                throw new Exception("La cantidad de colaboradores debe ser mayor que cero.");

            if (dto.FechaHoraRegreso <= dto.FechaHoraSalida)
                throw new Exception("La hora de regreso debe ser mayor que la hora de salida.");

            solicitud.AreaSolicitante = dto.AreaSolicitante;
            solicitud.CantidadColaboradores = dto.CantidadColaboradores;
            solicitud.FechaHoraSalida = dto.FechaHoraSalida;
            solicitud.FechaHoraRegreso = dto.FechaHoraRegreso;
            solicitud.Destino = dto.Destino;
            solicitud.Motivo = dto.Motivo;
            solicitud.Estado = dto.Estado;

            _repository.Update(solicitud);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var solicitud = await _repository.GetByIdAsync(id);

            if (solicitud == null)
                return false;

            _repository.Delete(solicitud);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
