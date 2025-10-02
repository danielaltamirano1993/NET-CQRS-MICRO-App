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
            // La lógica de cálculo de ItemsAltaRelevancia se mueve aquí para ser visible en C#.
            var usuarios = await _context.UsuarioReferencia
                .Where(u => u.Activo)
                .Select(u => new
                {
                    u.UsuarioId,
                    u.ItemsPendientes,
                    u.ItemsCompletados,
                    // Subconsulta para contar ítems de Alta Relevancia Pendientes (Saturación)
                    ItemsAltaRelevancia = _context.ItemTrabajo
                        .Count(i => i.UsuarioAsignado == u.UsuarioId
                                 && i.Estado == "Pendiente"
                                 && i.Relevancia == 2) // Relevancia = 2 es Alta
                })
                .ToListAsync();

            // Definición de Urgencia (menos de tres días desde la fecha actual)
            var fechaUrgente = DateTime.UtcNow.AddDays(3);

            // 🛑 PRIORIDAD 1: Urgencia (Fecha de entrega próxima a vencer)
            if (fechaEntrega < fechaUrgente)
            {
                // Asignar al usuario con menor cantidad de ítems pendientes, independientemente de la relevancia.
                return usuarios
                    .OrderBy(u => u.ItemsPendientes)
                    .ThenBy(u => u.ItemsCompletados)
                    .Select(u => (int?)u.UsuarioId)
                    .FirstOrDefault();
            }

            // 🛑 PRIORIDAD 2: Relevancia y Saturación
            if (relevancia == 2) // Alta Relevancia
            {
                // Filtrar usuarios saturados (más de 3 ítems altamente relevantes pendientes)
                var noSaturados = usuarios
                    .Where(u => u.ItemsAltaRelevancia <= 3)
                    .ToList();

                // Asignar al usuario no saturado con menor lista de pendientes
                return noSaturados
                    .OrderBy(u => u.ItemsPendientes)
                    .ThenBy(u => u.ItemsCompletados)
                    .Select(u => (int?)u.UsuarioId)
                    .FirstOrDefault();
            }
            else // Baja Relevancia (relevancia = 1)
            {
                // Asignar al usuario con menor lista de pendientes (sin filtro de saturación)
                return usuarios
                    .OrderBy(u => u.ItemsPendientes)
                    .ThenBy(u => u.ItemsCompletados)
                    .Select(u => (int?)u.UsuarioId)
                    .FirstOrDefault();
            }
        }
    }
}