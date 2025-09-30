using MediatR;
using Microsoft.EntityFrameworkCore;
using Microservicio.Items.API.Infrastructure;

namespace Microservicio.Items.API.App.Commands.CambiarEstadoItem
{
    public class CambiarEstadoItemCommandHandler
        : IRequestHandler<CambiarEstadoItemCommand, bool>
    {
        private readonly ItemDbContext _context;

        public CambiarEstadoItemCommandHandler(ItemDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            CambiarEstadoItemCommand request,
            CancellationToken cancellationToken
        )
        {
            var item = await _context.ItemTrabajo
                .FirstOrDefaultAsync(
                    i => i.ItemId == request.ItemId,
                    cancellationToken
                );

            if (item == null)
                return false;

            var estadoAnterior = item.Estado;
            var usuarioId = item.UsuarioAsignado;

            var estadosPermitidos = new[] {
                "Pendiente",
                "Completado"
            };

            if (!estadosPermitidos
                .Contains(
                    request.NuevoEstado
                )
            )
                throw new ArgumentException(
                    $"El estado '{request.NuevoEstado}' no es válido."
                );

            var usuario = await _context.UsuarioReferencia.FindAsync(usuarioId);

            if (usuario != null)
            {
                bool estadoAnteriorEraCompletado = estadoAnterior == "Completado";
                bool nuevoEstadoEsCompletado = request.NuevoEstado == "Completado";

                if (!estadoAnteriorEraCompletado && nuevoEstadoEsCompletado)
                {
                    usuario.ItemsPendientes = Math.Max(0, usuario.ItemsPendientes - 1);
                    usuario.ItemsCompletados += 1;
                }
                else if (estadoAnteriorEraCompletado && !nuevoEstadoEsCompletado)
                {
                    usuario.ItemsCompletados = Math.Max(0, usuario.ItemsCompletados - 1);
                    usuario.ItemsPendientes += 1;
                }
            }

            item.Estado = request.NuevoEstado;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}