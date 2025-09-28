using MediatR;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microservicio.Items.API.Application.Commands
{
    public class CrearItemTrabajoCommandHandler : IRequestHandler<CrearItemTrabajoCommand, int>
    {
        private readonly ItemDbContext _context;

        public CrearItemTrabajoCommandHandler(ItemDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CrearItemTrabajoCommand request, CancellationToken cancellationToken)
        {
            var usuarioRef = await _context.UsuarioReferencia.FindAsync(request.UsuarioReferenciaId);
            if (usuarioRef == null)
                throw new Exception($"El UsuarioReferencia con Id {request.UsuarioReferenciaId} no existe.");

            var item = new ItemTrabajo
            {
                Titulo = request.Titulo,
                Descripcion = request.Descripcion,
                FechaCreacion = DateTime.UtcNow,
                FechaEntrega = request.FechaEntrega,
                Relevancia = request.Relevancia,  // 1 = Baja, 2 = Alta
                Estado = "Pendiente",
                UsuarioAsignado = request.UsuarioReferenciaId
            };

            _context.ItemTrabajo.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            var historial = new HistorialAsignacion
            {
                ItemId = item.ItemId,
                UsuarioId = request.UsuarioReferenciaId,
                FechaAsignacion = DateTime.UtcNow,
                EstadoAsignacion = "Activa"
            };

            _context.HistorialAsignacion.Add(historial);
            await _context.SaveChangesAsync(cancellationToken);

            return item.ItemId;
        }
    }
}
