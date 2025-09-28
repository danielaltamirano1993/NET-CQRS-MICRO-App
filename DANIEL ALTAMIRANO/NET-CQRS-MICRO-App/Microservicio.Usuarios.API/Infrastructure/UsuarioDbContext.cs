using Microsoft.EntityFrameworkCore;
using Microservicio.Usuarios.API.Domain;

namespace Microservicio.Usuarios.API.Infrastructure
{
    public class UsuarioDbContext : DbContext
    {
        public UsuarioDbContext(
            DbContextOptions<UsuarioDbContext> options
        ) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioEstadistica> Estadisticas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<UsuarioEstadistica>().ToTable("UsuarioEstadistica");
            modelBuilder.Entity<UsuarioEstadistica>(
                entity =>
                {
                    entity.HasKey(e => e.EstadisticaId);
                    entity.HasOne(e => e.Usuario)
                          .WithOne(u => u.Estadistica)
                          .HasForeignKey<UsuarioEstadistica>(e => e.UsuarioId)
                          .OnDelete(DeleteBehavior.Cascade);
                    entity.Property(e => e.ItemsPendientes)
                          .HasDefaultValue(0);
                    entity.Property(e => e.ItemsCompletados)
                          .HasDefaultValue(0);
                    entity.Property(e => e.UltimaActualizacion)
                          .HasDefaultValueSql("GETDATE()");
                }
            );

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.UsuarioId);
            });
        }
    }
}
