using Microsoft.EntityFrameworkCore;
using Microservicio.Items.API.Domain;

namespace Microservicio.Items.API.Infrastructure
{
    public class ItemDbContext : DbContext
    {
        public ItemDbContext(DbContextOptions<ItemDbContext> options)
            : base(options) { }

        public DbSet<ItemTrabajo> ItemTrabajo { get; set; }
        public DbSet<HistorialAsignacion> HistorialAsignacion { get; set; }
        public DbSet<UsuarioReferencia> UsuarioReferencia { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuarioReferencia>()
                .HasMany(u => u.ItemsAsignados)
                .WithOne(i => i.Usuario)
                .HasForeignKey(i => i.UsuarioAsignado);

            modelBuilder.Entity<UsuarioReferencia>()
                .HasMany(u => u.Historiales)
                .WithOne(h => h.Usuario)
                .HasForeignKey(h => h.UsuarioId);

            modelBuilder.Entity<ItemTrabajo>()
                .HasMany(i => i.Historiales)
                .WithOne(h => h.Item)
                .HasForeignKey(h => h.ItemId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
