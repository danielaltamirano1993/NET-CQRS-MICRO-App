using Microservicio.Items.API.App.Dto;
using Microservicio.Items.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Items.API.Infrastructure
{
    public class ItemDbContext : DbContext
    {
        public ItemDbContext(DbContextOptions<ItemDbContext> options)
            : base(options) { }

        public DbSet<ItemTrabajo> ItemTrabajo { get; set; }
        public DbSet<HistorialAsignacion> HistorialAsignacion { get; set; }
        public DbSet<UsuarioReferencia> UsuarioReferencia { get; set; }
        public DbSet<ItemTrabajoSqlResult> ItemTrabajoSqlResults { get; set; } //dto
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

            modelBuilder.Entity<ItemTrabajoSqlResult>().
                HasNoKey();

            modelBuilder.Entity<HistorialAsignacion>(entity =>
            {
                entity.HasKey(h => h.HistorialId);

                entity.HasOne(h => h.Item)
                      .WithMany(i => i.Historiales)
                      .HasForeignKey(h => h.ItemId) 
                      .IsRequired();

                entity.HasOne(h => h.Usuario)
                      .WithMany(u => u.Historiales)
                      .HasForeignKey(h => h.UsuarioId)
                      .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
