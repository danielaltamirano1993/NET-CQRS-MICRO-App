using Microservicio.Items.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservicio.Items.API.Infrastructure
{
    public class HistorialAsignacionConfiguration : IEntityTypeConfiguration<HistorialAsignacion>
    {
        public void Configure(EntityTypeBuilder<HistorialAsignacion> builder)
        {
            builder.ToTable("HistorialAsignacion");

            builder.HasKey(h => h.HistorialId);

            builder.Property(h => h.Comentarios)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(h => h.FechaAsignacion)
                .IsRequired();

            builder.HasOne<ItemTrabajo>()
                   .WithMany(i => i.Historiales)
                   .HasForeignKey(h => h.ItemTrabajoId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(h => h.Usuario)
                   .WithMany(u => u.Historiales) 
                   .HasForeignKey(h => h.UsuarioId) 
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}
