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
        public DbSet<ItemTrabajoSqlResult> ItemTrabajoSqlResults { get; set; } // DTO

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItemDbContext).Assembly);
            modelBuilder.Entity<ItemTrabajoSqlResult>()
                         .HasNoKey()
                         .ToView(null);

            base.OnModelCreating(modelBuilder);
        }
    }
}
