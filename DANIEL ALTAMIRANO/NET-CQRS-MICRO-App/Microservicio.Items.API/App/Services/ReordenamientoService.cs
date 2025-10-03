using Microservicio.Items.API.App.Services.Contracts;
using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microservicio.Items.API.Domain;

namespace Microservicio.Items.API.App.Services
{
    public class ReordenamientoService : IReordenamientoService
    {
        private readonly ItemDbContext _context;

        public ReordenamientoService(ItemDbContext context)
        {
            _context = context;
        }

        public async Task ReordenarItemsPendientesAsync(int usuarioId)
        {
            var itemsPendientes = await _context.ItemTrabajo
                .Where(i => i.UsuarioAsignado == usuarioId && i.Estado == "Pendiente")
                .ToListAsync();

            var itemsOrdenados = itemsPendientes
                .OrderByDescending(i => i.Relevancia)
                .ThenBy(i => i.FechaEntrega.Date)
                .ThenBy(i => i.FechaEntrega)
                .ThenByDescending(i => -i.FechaCreacion.Ticks)
                .ToList();

            int nuevoOrden = 1;
            foreach (var item in itemsOrdenados)
            {
                item.Orden = nuevoOrden++;
            }

            await _context.SaveChangesAsync();
        }
    }
}
