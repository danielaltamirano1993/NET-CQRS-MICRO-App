using Microsoft.EntityFrameworkCore;
using Microservicio.Items.API.Domain;

namespace Microservicio.Items.API.Infrastructure
{
    public class ItemDbContext : DbContext
    {
        public ItemDbContext(DbContextOptions<ItemDbContext> options) : base(options) { }

        public DbSet<ItemTrabajo> ItemTrabajo { get; set; }
        public DbSet<HistorialAsignacion> HistorialAsignacion { get; set; }
        public DbSet<UsuarioReferencia> UsuarioReferencia { get; set; }
    }
}
