using MediatR;
using Microsoft.EntityFrameworkCore;
using Microservicio.Items.API.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using Microservicio.Items.API.Domain;

namespace Microservicio.Items.API.App.Commands.CompletarItemTrabajo
{
    public class CompletarItemTrabajoCommandHandler
        : IRequestHandler<CompletarItemTrabajoCommand, bool>
    {
        private readonly ItemDbContext _context;

        public CompletarItemTrabajoCommandHandler(
            ItemDbContext context
        )
        {
            _context = context;
        }

        public async Task<bool> Handle(
            CompletarItemTrabajoCommand request,
            CancellationToken cancellationToken
        )
        {
            var item = await _context.ItemTrabajo
                .Include(i => i.Historiales)
                .FirstOrDefaultAsync(
                    i => i.ItemId == request.ItemId,
                    cancellationToken
                );

            if (item == null) return false;

            item.Estado = "Completado";
            item.Historiales.Add(
                HistorialAsignacion.Crear(
                    itemTrabajoId: item.ItemId,
                    usuarioId: request.UsuarioReferenciaId,
                    comentarios: "Ítem completado" 
                )
            );

            await _context.SaveChangesAsync(
                cancellationToken
            );

            return true;
        }
    }
}