using Microsoft.EntityFrameworkCore;
using SistemaTransporte.Domain.Entities;

namespace SistemaTransporte.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Conductor> Conductores { get; set; }
        public DbSet<SolicitudTransporte> SolicitudesTransporte { get; set; }
        public DbSet<Asignacion> Asignaciones { get; set; }
        public DbSet<Viaje> Viajes { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<ConsumoCombustible> ConsumosCombustible { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConsumoCombustible>()
                .Property(c => c.Costo)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ConsumoCombustible>()
                .Property(c => c.Galones)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ConsumoCombustible>()
                .Property(c => c.KilometrosRecorridos)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Mantenimiento>()
                .Property(m => m.Costo)
                .HasPrecision(18, 2);
        }
    }
}