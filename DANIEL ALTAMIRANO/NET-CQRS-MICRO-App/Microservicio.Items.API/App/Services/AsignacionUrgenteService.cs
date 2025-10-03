using Microservicio.Items.API.App.Services.Contracts;
using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microservicio.Items.API.Domain;

namespace Microservicio.Items.API.App.Services
{
    public interface IAsignacionUrgenteService
    {
        Task AsignarItemsUrgentesAsync();
    }

    internal class UsuarioCarga
    {
        public int UsuarioId { get; init; }
        public int LimiteItems { get; init; }
        public int CargaActual { get; set; }
    }

    public class AsignacionUrgenteService : IAsignacionUrgenteService
    {
        private readonly ItemDbContext _context;

        public AsignacionUrgenteService(ItemDbContext context)
        {
            _context = context;
        }

        public async Task AsignarItemsUrgentesAsync()
        {
            var fechaLimiteUrgencia = DateTime.Today.AddDays(3);

            var itemsUrgentes = await _context.ItemTrabajo
                .Where(i => (i.UsuarioAsignado == null || i.UsuarioAsignado == 0)
                            && i.Estado != "Completado"
                            && i.FechaEntrega.Date < fechaLimiteUrgencia)
                .OrderByDescending(i => i.Relevancia)
                .ThenBy(i => i.FechaCreacion)
                .ToListAsync();

            var usuarios = await _context.UsuarioReferencia.ToListAsync();

            var itemsPendientesPorUsuario = await _context.ItemTrabajo
                .Where(i => i.Estado == "Pendiente" && i.UsuarioAsignado != 0)
                .GroupBy(i => i.UsuarioAsignado)
                .Select(g => new { UsuarioId = g.Key.Value, CantidadPendiente = g.Count() })
                .ToDictionaryAsync(x => x.UsuarioId, x => x.CantidadPendiente);

            var usuariosOrdenadosPorCarga = usuarios
                .Select(u => new UsuarioCarga
                {
                    UsuarioId = u.UsuarioId,
                    LimiteItems = u.LimiteItems,
                    CargaActual = itemsPendientesPorUsuario.GetValueOrDefault(u.UsuarioId, 0)
                })
                .OrderBy(u => u.CargaActual)
                .ThenBy(u => u.UsuarioId)
                .Where(u => u.CargaActual < u.LimiteItems)
                .ToList();

            var usuariosIndex = 0;
            var cambiosRealizados = false;

            foreach (var item in itemsUrgentes)
            {
                if (usuariosOrdenadosPorCarga.Count == 0)
                    break;

                var currentIndex = usuariosIndex % usuariosOrdenadosPorCarga.Count;
                var usuarioDisponible = usuariosOrdenadosPorCarga[currentIndex];

                if (usuarioDisponible.CargaActual < usuarioDisponible.LimiteItems)
                {
                    item.UsuarioAsignado = usuarioDisponible.UsuarioId;
                    item.Estado = "Pendiente";
                    cambiosRealizados = true;
                    usuarioDisponible.CargaActual++;
                    usuariosIndex++;
                }
                else
                {
                    usuariosIndex++;
                }
            }

            if (cambiosRealizados)
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
