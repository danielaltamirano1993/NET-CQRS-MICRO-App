using MediatR;
using Microservicio.Items.API.App.Services;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Necesario para BeginTransactionAsync

namespace Microservicio.Items.API.App.Commands
{
    public class CrearItemTrabajoCommandHandler : IRequestHandler<CrearItemTrabajoCommand, int>
    {
        private readonly ItemDbContext _context;
        private readonly AsignacionService _asignacionService;

        public CrearItemTrabajoCommandHandler(ItemDbContext context, AsignacionService asignacionService)
        {
            _context = context;
            _asignacionService = asignacionService;
        }

        public async Task<int> Handle(CrearItemTrabajoCommand request, CancellationToken cancellationToken)
        {
            // 1. Lógica de SELECCIÓN (Refactorizada del SP)
            int? usuarioId = await _asignacionService.SeleccionarUsuarioAsignacionAsync(
                request.FechaEntrega,
                request.Relevancia
            );

            if (!usuarioId.HasValue)
                throw new Exception("No hay usuario disponible para la asignación según las reglas de negocio.");


            // 2. Lógica TRANSACCIONAL (Refactorizada del SP)

            // Usar una transacción explícita para emular el TRY/CATCH/TRANSACTION del SP
            await using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Obtener el usuario para la actualización del contador
                var usuario = await _context.UsuarioReferencia.FindAsync(usuarioId.Value);

                if (usuario == null)
                {
                    // Esto no debería pasar si el servicio funciona, pero es buena práctica.
                    throw new Exception($"El usuario seleccionado (ID: {usuarioId.Value}) no existe.");
                }

                // a. Crear ItemTrabajo
                var item = new ItemTrabajo
                {
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaCreacion = DateTime.UtcNow,
                    FechaEntrega = request.FechaEntrega,
                    Relevancia = request.Relevancia,
                    Estado = "Pendiente",
                    UsuarioAsignado = usuarioId.Value // Asignación directa
                };

                _context.ItemTrabajo.Add(item);

                // b. Actualizar Contadores (Solo el nuevo usuario)
                usuario.ItemsPendientes += 1;

                // c. Crear Historial (Se vinculará al ItemId después del SaveChanges)
                var historial = new HistorialAsignacion
                {
                    // ItemId se llenará correctamente después del SaveChanges (Identity Insert)
                    UsuarioId = usuarioId.Value,
                    FechaAsignacion = DateTime.UtcNow,
                    EstadoAsignacion = "Activa",
                    Item = item // Enlazar el objeto Item para que EF gestione la FK
                };
                _context.HistorialAsignacion.Add(historial);

                // Guardar todos los cambios (INSERT Item, UPDATE Usuario, INSERT Historial)
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return item.ItemId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw; // Relanza la excepción para que sea capturada por el controlador.
            }
        }
    }
}