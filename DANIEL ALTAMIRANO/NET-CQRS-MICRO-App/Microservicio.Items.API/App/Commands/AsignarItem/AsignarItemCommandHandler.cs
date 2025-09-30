using MediatR;
using Microservicio.Items.API.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Microservicio.Items.API.App.Commands.AsignarItem
{
    public class AsignarItemCommandHandler : IRequestHandler<AsignarItemCommand, int> 
    {
        private readonly ItemDbContext _context;

        public AsignarItemCommandHandler(
            ItemDbContext context
        ) => _context = context;

        public async Task<int> Handle(
            AsignarItemCommand request, 
            CancellationToken cancellationToken
        )
        {
            var itemIdParam = new SqlParameter(
                "@ItemId", 
                SqlDbType.Int
            ) 
            { 
                Value = request.ItemId 
            };

            var usuarioIdOutputParam = new SqlParameter(
                "@UsuarioAsignadoId", 
                SqlDbType.Int
            )
            {
                Direction = ParameterDirection.Output
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_AsignarItem " +
                         "@ItemId, " +
                         "@UsuarioAsignadoId OUTPUT",
                    new[] { 
                            itemIdParam, 
                            usuarioIdOutputParam 
                    },
                    cancellationToken
                );

                if (usuarioIdOutputParam.Value == DBNull.Value || 
                    (int)usuarioIdOutputParam.Value == 0
                )
                {
                    throw new Exception(
                        "El sistema no encontró un usuario disponible " +
                        "para la asignación automática."
                    );
                }

                return (int)usuarioIdOutputParam.Value;
            }
            catch (SqlException ex)
            {
                throw new Exception(
                    $"Error de base de datos al asignar el ítem: " +
                    $"{ex.Message}",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error inesperado: " +
                    $"{ex.Message}",
                    ex
                );
            }
        }
    }
}