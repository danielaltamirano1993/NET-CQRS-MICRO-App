using Microservicio.Items.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservicio.Items.API.Infrastructure.Configuration
{
    public class HistorialAsignacionConfiguration : IEntityTypeConfiguration<HistorialAsignacion>
    {
        public void Configure(EntityTypeBuilder<HistorialAsignacion> builder)
        {
            builder.HasKey(h => h.HistorialId);
            builder.HasOne<ItemTrabajo>() 
                   .WithMany()
                   .HasForeignKey(h => h.ItemTrabajoId)
                   .IsRequired();

            builder.HasOne<UsuarioReferencia>()
                   .WithMany()
                   .HasForeignKey(h => h.UsuarioId)
                   .IsRequired();
        }
    }
}