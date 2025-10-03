using MediatR;
using Microservicio.Items.API.App.Services.Contracts;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Microservicio.Items.API.App.Commands.CrearItemTrabajo
{
    public class CrearItemTrabajoCommandHandler : IRequestHandler<CrearItemTrabajoCommand, int>
    {
        private readonly ItemDbContext _context;
        private readonly IAsignacionService _asignacionService;
        private readonly IReordenamientoService _reordenamientoService;

        public CrearItemTrabajoCommandHandler(
            ItemDbContext context,
            IAsignacionService asignacionService,
            IReordenamientoService reordenamientoService
        )
        {
            _context = context;
            _asignacionService = asignacionService;
            _reordenamientoService = reordenamientoService;
        }

        public async Task<int> Handle(
            CrearItemTrabajoCommand request,
            CancellationToken cancellationToken
        )
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            int itemId = -1;
            UsuarioReferencia? usuarioFinalAsignado = null;

            try
            {
                if (request.UsuarioAsignado.HasValue && request.UsuarioAsignado.Value > 0)
                {
                    usuarioFinalAsignado = await _context.UsuarioReferencia
                        .FirstOrDefaultAsync(u => u.UsuarioId == request.UsuarioAsignado.Value && u.Activo, cancellationToken);

                    if (usuarioFinalAsignado == null)
                    {
                        Console.WriteLine($"[ADVERTENCIA] UsuarioAsignado ({request.UsuarioAsignado.Value}) no encontrado o inactivo. Procediendo con asignación automática.");
                    }
                }

                if (usuarioFinalAsignado == null)
                {
                    try
                    {
                        usuarioFinalAsignado = await _asignacionService.AsignarUsuario(
                            request.Relevancia,
                            ItemTrabajo.Crear(
                                request.Titulo,
                                request.Descripcion,
                                request.FechaEntrega,
                                request.Relevancia
                            )
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ADVERTENCIA] AsignacionService falló. Intentando plan de respaldo: {ex.Message}.");
                    }
                }

                if (usuarioFinalAsignado == null)
                {
                    usuarioFinalAsignado = await _context.UsuarioReferencia
                        .Where(u => u.Activo)
                        .OrderBy(u => u.ItemsPendientes)
                        .FirstOrDefaultAsync(cancellationToken);
                }

                if (usuarioFinalAsignado == null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new Exception("No hay usuario disponible para la asignación. Verifique que la tabla 'UsuarioReferencia' contenga al menos un usuario ACTIVO.");
                }

                if (usuarioFinalAsignado.ItemsPendientes >= usuarioFinalAsignado.LimiteItems)
                {
                    Console.WriteLine($"[RECHAZO] Ítem rechazado: '{request.Titulo}'. Usuario {usuarioFinalAsignado.UsuarioId} ha alcanzado su límite de {usuarioFinalAsignado.LimiteItems} ítems pendientes.");
                    await transaction.RollbackAsync(cancellationToken);
                    return -1;
                }

                var nuevoItem = ItemTrabajo.Crear(
                    request.Titulo,
                    request.Descripcion,
                    request.FechaEntrega,
                    request.Relevancia
                );

                nuevoItem.AsignarUsuario(usuarioFinalAsignado.UsuarioId);
                _context.ItemTrabajo.Add(nuevoItem);

                await _context.SaveChangesAsync(cancellationToken);
                itemId = nuevoItem.ItemId;

                _context.HistorialAsignacion.Add(
                    HistorialAsignacion.Crear(
                        nuevoItem.ItemId,
                        usuarioFinalAsignado.UsuarioId,
                        "Creado y asignado automáticamente"
                    )
                );

                await _context.SaveChangesAsync(cancellationToken);

                await _context.UsuarioReferencia
                    .Where(u => u.UsuarioId == usuarioFinalAsignado.UsuarioId)
                    .ExecuteUpdateAsync(
                        setters => setters
                            .SetProperty(u => u.ItemsPendientes, u => u.ItemsPendientes + 1)
                            .SetProperty(u => u.UltimaActualizacion, u => DateTime.UtcNow),
                        cancellationToken
                    );

                await _reordenamientoService.ReordenarItemsPendientesAsync(usuarioFinalAsignado.UsuarioId);

                await transaction.CommitAsync(cancellationToken);

                return itemId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Console.WriteLine($"ERROR FATAL DURANTE LA CREACIÓN DEL ÍTEM: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return -1;
            }
        }
    }
}
