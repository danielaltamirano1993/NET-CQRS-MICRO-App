using MediatR;
using Microservicio.Items.API.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Microservicio.Items.API.App.Commands.AsignarItem
{
    public class AsignarItemCommandHandler : IRequestHandler<AsignarItemCommand, bool>
    {
        private readonly ItemDbContext _context;

        public AsignarItemCommandHandler(ItemDbContext context) => _context = context;

        public async Task<bool> Handle(AsignarItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.ItemTrabajo.FindAsync(request.ItemId);
            if (item == null)
                throw new Exception($"Item con Id {request.ItemId} no existe.");

            var param = new SqlParameter("@ItemId", SqlDbType.Int) { Value = request.ItemId };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_AsignarItem @ItemId",
                    new[] { param },
                    cancellationToken
                );

                return true;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al asignar el ítem: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inesperado: {ex.Message}", ex);
            }
        }
    }
}
