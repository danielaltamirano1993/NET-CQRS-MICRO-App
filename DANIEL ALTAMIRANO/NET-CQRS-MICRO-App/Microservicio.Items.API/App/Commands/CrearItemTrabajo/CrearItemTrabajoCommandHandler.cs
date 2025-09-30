using MediatR;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
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
            var item = new ItemTrabajo
            {
                Titulo = request.Titulo,
                Descripcion = request.Descripcion,
                FechaCreacion = DateTime.UtcNow,
                FechaEntrega = request.FechaEntrega,
                Relevancia = request.Relevancia,
                Estado = "Pendiente", 
                UsuarioAsignado = null 
            };

            _context.ItemTrabajo.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            int newItemId = item.ItemId;
            int usuarioAsignadoId = 0;

            var itemIdParam = new SqlParameter("@ItemId", SqlDbType.Int) { Value = newItemId };
            var outputParam = new SqlParameter("@UsuarioAsignadoId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_AsignarItem @ItemId, @UsuarioAsignadoId OUTPUT",
                    new[] { itemIdParam, outputParam },
                    cancellationToken
                );

                if (outputParam.Value != DBNull.Value)
                {
                    usuarioAsignadoId = (int)outputParam.Value;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al asignar automáticamente el ítem: {ex.Message}", ex);
            }

            return newItemId;
        }
    }
}