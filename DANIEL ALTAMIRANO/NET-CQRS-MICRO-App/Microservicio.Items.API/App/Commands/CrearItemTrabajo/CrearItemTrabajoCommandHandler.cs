using MediatR;
using Microservicio.Items.API.App.Services.Contracts;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microservicio.Items.API.App.Commands.CrearItemTrabajo
{
    public class CrearItemTrabajoCommandHandler : IRequestHandler<CrearItemTrabajoCommand, int>
    {
        private readonly ItemDbContext _context;
        private readonly IAsignacionService _asignacionService;

        public CrearItemTrabajoCommandHandler(
            ItemDbContext context, 
            IAsignacionService asignacionService
        )
        {
            _context = context;
            _asignacionService = asignacionService;
        }

        public async Task<int> Handle(
            CrearItemTrabajoCommand request, 
            CancellationToken cancellationToken
        )
        {
            var item = ItemTrabajo.Crear(
                request.Titulo,
                request.Descripcion,
                request.FechaEntrega,
                request.Relevancia);

            UsuarioReferencia usuarioAsignado = null;
            try
            {
                usuarioAsignado = await _asignacionService.AsignarUsuario(
                    request.Relevancia,
                    item
                );
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(
                    $"[ADVERTENCIA] AsignacionService falló: " +
                    $"{ex.Message}. ");
            }

            if (usuarioAsignado == null)
            {
                usuarioAsignado = await _context.UsuarioReferencia
                    .OrderBy(u => u.ItemsPendientes)
                    .FirstOrDefaultAsync(cancellationToken);

                if (usuarioAsignado == null)
                {
                    usuarioAsignado = await _context.UsuarioReferencia
                        .FirstOrDefaultAsync(cancellationToken);
                }
            }

            if (usuarioAsignado == null)
            {
                throw new System.Exception(
                    "No hay usuario disponible para la asignación según las reglas de negocio. " +
                    "Por favor, verifique que la tabla 'UsuarioReferencia' contenga al menos un usuario.");
            }

            item.AsignarUsuario(
                usuarioAsignado.UsuarioId
            );

            item.AgregarHistorial(
                HistorialAsignacion.Crear(
                    item.ItemId, 
                    usuarioAsignado.UsuarioId, 
                    "Creado y asignado automáticamente"
                )
            );

            usuarioAsignado.ItemsPendientes++;
            _context.ItemTrabajo.Add(item);
            _context.UsuarioReferencia.Update(usuarioAsignado);
            await _context.SaveChangesAsync(cancellationToken);
            return item.ItemId;
        }
    }
}
