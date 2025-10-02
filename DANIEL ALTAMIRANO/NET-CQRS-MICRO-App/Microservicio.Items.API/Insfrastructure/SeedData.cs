using Microservicio.Items.API.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Microservicio.Items.API.Infrastructure
{
    public static class SeedData
    {
        public static async Task EnsureSeedData(ItemDbContext context)
        {
            if (!await context.UsuarioReferencia.AnyAsync())
            {
                var usuarios = new[]
                {
                    new UsuarioReferencia
                    {
                        UsuarioId = 1,
                        NombreUsuario = "Developer 1",
                        ItemsPendientes = 0,
                        ItemsCompletados = 0,
                        Activo = true
                    },
                    new UsuarioReferencia
                    {
                        UsuarioId = 2,
                        NombreUsuario = "Analyst 2",
                        ItemsPendientes = 0,
                        ItemsCompletados = 0,
                        Activo = true
                    },
                    new UsuarioReferencia
                    {
                        UsuarioId = 3,
                        NombreUsuario = "Tester 3",
                        ItemsPendientes = 0,
                        ItemsCompletados = 0,
                        Activo = true
                    },
                    new UsuarioReferencia
                    {
                        UsuarioId = 4,
                        NombreUsuario = "Inactive User",
                        ItemsPendientes = 0,
                        ItemsCompletados = 0,
                        Activo = false
                    }
                };

                await context.UsuarioReferencia.AddRangeAsync(usuarios);
                await context.SaveChangesAsync();
            }
        }
    }
}
