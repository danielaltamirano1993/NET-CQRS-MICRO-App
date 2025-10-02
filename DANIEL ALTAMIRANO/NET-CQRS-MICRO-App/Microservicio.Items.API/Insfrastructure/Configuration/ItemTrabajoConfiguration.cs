using Microservicio.Items.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservicio.Items.API.Infrastructure.Configuration
{
    public class ItemTrabajoConfiguration : IEntityTypeConfiguration<ItemTrabajo>
    {
        public void Configure(
            EntityTypeBuilder<ItemTrabajo> builder
        )
        {
            builder.HasOne(i => i.Usuario) 
                   .WithMany(u => u.ItemsAsignados)
                   .HasForeignKey(i => i.UsuarioAsignado)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}