using Microservicio.Items.API.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microservicio.Items.API.Infrastructure.Configuration
{
    public class UsuarioReferenciaConfiguration : IEntityTypeConfiguration<UsuarioReferencia>
    {
        public void Configure(
            EntityTypeBuilder<UsuarioReferencia> builder
        )
        {
            builder.HasKey(u => u.UsuarioId);
            builder.Property(u => u.UsuarioId)
                   .ValueGeneratedNever();
            builder.Property(u => u.NombreUsuario)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(u => u.Activo)
                   .IsRequired();
        }
    }
}