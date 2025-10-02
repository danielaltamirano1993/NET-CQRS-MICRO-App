using Microservicio.Items.API.App.Services.Contracts; 
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microservicio.Items.API.App.Services
{
    public class AsignacionService : IAsignacionService
    {
        private readonly ItemDbContext _context;

        public AsignacionService(ItemDbContext context)
        {
            _context = context;
        }

        public async Task<UsuarioReferencia> AsignarUsuario(
            byte relevancia, 
            ItemTrabajo item
        )
        {
            int? usuarioId = await SeleccionarUsuarioIdAsync(
                item.FechaEntrega, 
                relevancia
            );

            if (usuarioId.HasValue)
            {
                return await _context.UsuarioReferencia
                    .FirstOrDefaultAsync(
                    u => u.UsuarioId == usuarioId.Value
                );
            }
            return null;
        }
        
        private async Task<int?> SeleccionarUsuarioIdAsync(
            DateTime fechaEntrega, 
            byte relevancia
        )
        {
            var usuariosActivos = await _context.UsuarioReferencia
                .Where(u => u.Activo)
                .ToListAsync();

            if (!usuariosActivos.Any())
            {
                return null;
            }

            var altaRelevanciaPendientes = await _context.ItemTrabajo
                .Where(i => i.Estado == "Pendiente" && 
                            i.Relevancia == 2
                )
                .GroupBy(i => i.UsuarioAsignado)
                .Select(g => new { 
                                  UsuarioId = g.Key, 
                                  Count     = g.Count() 
                                 }
                )
                .ToDictionaryAsync(x => x.UsuarioId, x => x.Count);


            var fechaUrgente = DateTime.UtcNow
                                       .AddDays(3);
            int? usuarioId = null;

            var usuariosConCarga = usuariosActivos
                .Select(u => new
                {
                    u.UsuarioId,
                    u.ItemsPendientes,
                    u.ItemsCompletados,
                    ItemsAltaRelevancia = altaRelevanciaPendientes
                                          .GetValueOrDefault(u.UsuarioId)
                })
                .ToList();

            if (fechaEntrega < fechaUrgente)
            {
                usuarioId = usuariosConCarga
                    .OrderBy(u => u.ItemsPendientes)
                    .ThenBy(u => u.ItemsCompletados)
                    .Select(u => (int?)u.UsuarioId)
                    .FirstOrDefault();
            }

            else if (relevancia == 2)
            {
                var noSaturados = usuariosConCarga
                    .Where(u => u.ItemsAltaRelevancia <= 3)
                    .ToList();

                usuarioId = noSaturados
                    .OrderBy(u => u.ItemsPendientes)
                    .ThenBy(u => u.ItemsCompletados)
                    .Select(u => (int?)u.UsuarioId)
                    .FirstOrDefault();

                if (usuarioId == null)
                {
                    usuarioId = usuariosConCarga
                        .OrderBy(u => u.ItemsPendientes)
                        .ThenBy(u => u.ItemsCompletados)
                        .Select(u => (int?)u.UsuarioId)
                        .FirstOrDefault();
                }
            }

            else
            {
                usuarioId = usuariosConCarga
                    .OrderBy(u => u.ItemsPendientes)
                    .ThenBy(u => u.ItemsCompletados)
                    .Select(u => (int?)u.UsuarioId)
                    .FirstOrDefault();
            }

            return usuarioId;
        }
    }
}
