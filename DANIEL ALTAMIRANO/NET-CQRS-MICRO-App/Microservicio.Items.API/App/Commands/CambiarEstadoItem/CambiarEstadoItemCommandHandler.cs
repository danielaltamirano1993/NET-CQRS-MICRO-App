using MediatR;
using Microsoft.EntityFrameworkCore;
using Microservicio.Items.API.Infrastructure;
using Microservicio.Items.API.Domain;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microservicio.Items.API.App.Services.Contracts;

namespace Microservicio.Items.API.App.Commands.CambiarEstadoItem
{
    public class CambiarEstadoItemCommandHandler
        : IRequestHandler<CambiarEstadoItemCommand, bool>
    {
        private readonly ItemDbContext _context;
        private readonly IReordenamientoService _reordenamientoService;

        public CambiarEstadoItemCommandHandler(
            ItemDbContext context,
            IReordenamientoService reordenamientoService
        )
        {
            _context = context;
            _reordenamientoService = reordenamientoService;
        }

        public async Task<bool> Handle(
            CambiarEstadoItemCommand request,
            CancellationToken cancellationToken
        )
        {
            await using var transaccion = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var estadosPermitidos = new[] {
                    "Pendiente",
                    "Completado"
                };

                if (!estadosPermitidos.Contains(request.NuevoEstado))
                    throw new ArgumentException(
                        $"El estado '{request.NuevoEstado}' no es válido."
                    );

                var item = await _context.ItemTrabajo.FindAsync(
                    new object[] { request.ItemId },
                    cancellationToken
                );

                if (item == null)
                {
                    await transaccion.RollbackAsync(cancellationToken);
                    return false;
                }

                var estadoAnterior = item.Estado;

                if (estadoAnterior == request.NuevoEstado)
                {
                    await transaccion.CommitAsync(cancellationToken);
                    return true;
                }

                var tieneUsuarioAsignado = item.UsuarioAsignado.HasValue;

                if (tieneUsuarioAsignado)
                {
                    var usuarioId = item.UsuarioAsignado.GetValueOrDefault();
                    bool cambioEstadoCompletado = (estadoAnterior != "Completado" && request.NuevoEstado == "Completado") ||
                                                  (estadoAnterior == "Completado" && request.NuevoEstado != "Completado");

                    if (cambioEstadoCompletado)
                    {
                        _context.HistorialAsignacion.Add(new HistorialAsignacion
                        {
                            ItemTrabajoId = item.ItemId,
                            UsuarioId = usuarioId,
                            FechaAsignacion = DateTime.UtcNow,
                            Comentarios = request.NuevoEstado == "Completado" ?
                                $"Estado cambiado de '{estadoAnterior}' a '{request.NuevoEstado}'. (Completado)" :
                                $"Estado cambiado de '{estadoAnterior}' a '{request.NuevoEstado}'. (Reabierto)"
                        });

                        await _reordenamientoService.ReordenarItemsPendientesAsync(usuarioId);
                    }
                }

                item.Estado = request.NuevoEstado;

                await _context.SaveChangesAsync(cancellationToken);
                await transaccion.CommitAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                await transaccion.RollbackAsync(cancellationToken);

                System.Diagnostics.Debug.WriteLine($"ERROR FATAL DURANTE LA TRANSACCIÓN: {ex.Message}");

                return false;
            }
        }
    }
}
