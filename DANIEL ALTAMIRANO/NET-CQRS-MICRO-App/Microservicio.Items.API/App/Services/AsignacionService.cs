using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microservicio.Items.API.App.Services
{
    public class AsignacionService
    {
        private readonly ItemDbContext _context;

        public AsignacionService(ItemDbContext context)
        {
            _context = context;
        }

        public async Task<int?> SeleccionarUsuarioAsignacionAsync(DateTime fechaEntrega, byte relevancia)
        {
            // 1. Obtener datos de UsuarioReferencia activos.
            var usuariosActivos = await _context.UsuarioReferencia
                .Where(u => u.Activo)
                .ToListAsync();

            if (!usuariosActivos.Any())
            {
                // Si no hay usuarios activos en absoluto, regresamos null.
                return null;
            }

            // 2. Obtener todos los ítems de Alta Relevancia (Relevancia = 2) pendientes
            // y agruparlos por usuario para calcular la saturación en memoria.
            var altaRelevanciaPendientes = await _context.ItemTrabajo
                .Where(i => i.Estado == "Pendiente" && i.Relevancia == 2)
                .GroupBy(i => i.UsuarioAsignado)
                .Select(g => new { UsuarioId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UsuarioId, x => x.Count);


            var fechaUrgente = DateTime.UtcNow.AddDays(3);
            int? usuarioId = null;

            // Mapear los usuarios activos con su conteo de ítems de Alta Relevancia
            var usuariosConCarga = usuariosActivos
                .Select(u => new
                {
                    u.UsuarioId,
                    u.ItemsPendientes,
                    u.ItemsCompletados,
                    ItemsAltaRelevancia = altaRelevanciaPendientes.GetValueOrDefault(u.UsuarioId)
                })
                .ToList();


            // 🛑 PRIORIDAD 1: Urgencia (FechaEntrega < 3 días)
            if (fechaEntrega < fechaUrgente)
            {
                // Asignar al usuario con menor carga pendiente
                usuarioId = usuariosConCarga
                    .OrderBy(u => u.ItemsPendientes)
                    .ThenBy(u => u.ItemsCompletados)
                    .Select(u => (int?)u.UsuarioId)
                    .FirstOrDefault();
            }

            // 🛑 PRIORIDAD 2: Relevancia Alta
            else if (relevancia == 2)
            {
                // Intento 1: Filtrar No Saturados (ItemsAltaRelevancia <= 3)
                var noSaturados = usuariosConCarga
                    .Where(u => u.ItemsAltaRelevancia <= 3)
                    .ToList();

                // Asignar al no saturado con menor lista de pendientes
                usuarioId = noSaturados
                    .OrderBy(u => u.ItemsPendientes)
                    .ThenBy(u => u.ItemsCompletados)
                    .Select(u => (int?)u.UsuarioId)
                    .FirstOrDefault();

                // FALLBACK: Si todos están saturados, se asigna al menos cargado de todos.
                if (usuarioId == null)
                {
                    usuarioId = usuariosConCarga
                        .OrderBy(u => u.ItemsPendientes)
                        .ThenBy(u => u.ItemsCompletados)
                        .Select(u => (int?)u.UsuarioId)
                        .FirstOrDefault();
                }
            }

            // 🛑 PRIORIDAD 3: Baja Relevancia (relevancia = 1)
            else // Relevancia = 1
            {
                // Asignar al usuario con menor lista de pendientes (sin filtro de saturación)
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
