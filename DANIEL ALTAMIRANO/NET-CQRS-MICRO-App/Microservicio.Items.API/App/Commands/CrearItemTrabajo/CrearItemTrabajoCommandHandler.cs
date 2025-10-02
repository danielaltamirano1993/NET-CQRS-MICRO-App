using MediatR;
using Microservicio.Items.API.App.Services;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 

namespace Microservicio.Items.API.App.Commands
{
    public class CrearItemTrabajoCommandHandler : IRequestHandler<CrearItemTrabajoCommand, int>
    {
        private readonly ItemDbContext _context;
        private readonly AsignacionService _asignacionService;

        public CrearItemTrabajoCommandHandler(
            ItemDbContext context, 
            AsignacionService asignacionService
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
            int? usuarioId = await _asignacionService.SeleccionarUsuarioAsignacionAsync(
                request.FechaEntrega,
                request.Relevancia
            );

            if (!usuarioId.HasValue)
                throw new Exception(
                    "No hay usuario disponible para la asignación según las reglas de negocio."
                );

            await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var usuario = await _context.UsuarioReferencia.FindAsync(usuarioId.Value);

                if (usuario == null)
                {
                    throw new Exception(
                        $"El usuario seleccionado (" +
                        $"ID: {usuarioId.Value}) no existe."
                    );
                }

                var item = new ItemTrabajo
                {
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaCreacion = DateTime.UtcNow,
                    FechaEntrega = request.FechaEntrega,
                    Relevancia = request.Relevancia,
                    Estado = "Pendiente",
                    UsuarioAsignado = usuarioId.Value 
                };

                _context.ItemTrabajo.Add(item);

                usuario.ItemsPendientes += 1;

                var historial = new HistorialAsignacion
                {
                    UsuarioId = usuarioId.Value,
                    FechaAsignacion = DateTime.UtcNow,
                    EstadoAsignacion = "Activa",
                    Item = item 
                };
                _context.HistorialAsignacion.Add(historial);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return item.ItemId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}