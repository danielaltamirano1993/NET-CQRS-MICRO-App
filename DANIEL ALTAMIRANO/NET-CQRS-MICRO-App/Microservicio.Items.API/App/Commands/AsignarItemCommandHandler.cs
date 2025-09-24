using MediatR;
using Microservicio.Items.API.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Microservicio.Items.API.Application.Commands
{
    public class AsignarItemCommandHandler : IRequestHandler<AsignarItemCommand, bool>
    {
        private readonly ItemDbContext _context;

        public AsignarItemCommandHandler(
            ItemDbContext context
        ) => _context = context;

        public async Task<bool> Handle(
            AsignarItemCommand request, 
            CancellationToken cancellationToken
        )
        {
            var param = new SqlParameter(
                "@ItemId",
                SqlDbType.Int
            ) { 
                Value = request.ItemId 
            };
            try
            { 
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC dbo.sp_AsignarItem @ItemId",
                    param, 
                    cancellationToken
                );
                return true;
            }
            catch (SqlException ex)
            {
                throw;
            }
        }
    }
}
