using Microsoft.EntityFrameworkCore;
using Microservicio.Usuarios.API.Domain;

namespace Microservicio.Usuarios.API.Infrastructure
{
    public class UsuarioDbContext : DbContext
    {
        public UsuarioDbContext(
            DbContextOptions<UsuarioDbContext> options
        ) : base( 
            options
        ) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioEstadistica> Estadisticas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                        .HasOne(u => u.Estadistica)
                        .WithOne(e => e.Usuario)
                        .HasForeignKey<UsuarioEstadistica>(e => e.UsuarioId);
        }
    }
}
